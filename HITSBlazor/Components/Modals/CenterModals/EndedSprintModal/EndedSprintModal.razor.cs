using ApexCharts;
using HITSBlazor.Components.Modals.Components.RightSideModaCollapselInfo;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Services.Tags;
using HITSBlazor.Utils.Mocks.Projects;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.CenterModals.EndedSprintModal
{
    public partial class EndedSprintModal
    {
        [Inject]
        private ITagService TagService { get; set; } = null!;

        [Parameter]
        public required Guid ProjectId { get; set; }

        [Parameter]
        public required Sprint CurrentSprint { get; set; }

        private bool _isLoading = true;
        private string _seacrhText = string.Empty;

        private List<Tag> _tags = [];
        private List<ProjectMember> _members = [];

        private HashSet<Tag> SelectedTags { get; set; } = [];
        private ProjectMember? SelectedMember { get; set; }

        private EndedSprintModalStatCategory _activeStatCategory = EndedSprintModalStatCategory.GeneralStat;
        private EndedSprintModalInfoCategory _activeInfoCategory = EndedSprintModalInfoCategory.Scores;

        private List<CollapseItem> _sprintData = [];

        private Models.Projects.Entities.Task? _selectedTask;
        private List<CollapseItem> _taskData = [];
        private List<TaskMovementLog> _taskLogs = [];

        private List<TableHeaderItem> _tableTaskLogHeader = 
        [
            new() { Text = "Статус",            InCentered = true,  ColumnClass = "col-3"   },
            new() { Text = "Дата вступления",   InCentered = true                           },
            new() { Text = "Дата окончания",    InCentered = true                           },
            new() { Text = "Продолжительность", InCentered = true                           }
        ];

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            _isLoading = true;

            _tags = await TagService.GetTagsAsync();
            _members = [.. MockProjects.GetProjectById(ProjectId)?.Members ?? []];

            _sprintData = 
            [
                new() { Title = "Описание", Data = CurrentSprint.Goal },
                //TODO: узнать потом у Сани что и как по отчетам
                new() { Title = "Отчет",    Data = "ну тут нужно брать наверно репорт спринта, при завершении" },
            ];

            _isLoading = false;
        }

        private void SeacrhTask(string value)
        {
            _seacrhText = value;
        }

        private void SelectTask(Models.Projects.Entities.Task task)
        {
            if (_selectedTask?.Id == task.Id) return;

            _selectedTask = task;
            _taskData = [new() { Title = "Описание", Data = task.Description }];
            _taskLogs = MockTaskMovementLogs.GetTaskMovementLogsByTaskId(task.Id);

            StateHasChanged();
        }

        private List<EndedSprintModalStatCategory> GetCategoryTabs()
        {
            var tabs = new List<EndedSprintModalStatCategory>
            {
                EndedSprintModalStatCategory.GeneralStat
            };

            if (_selectedTask is not null)
                tabs.Add(EndedSprintModalStatCategory.TaskStat);

            return tabs;
        }

        private async System.Threading.Tasks.Task ChangeCategory(EndedSprintModalStatCategory category)
        {
            _activeStatCategory = category;
        }

        private List<DateTime> GetSprintDays()
        {
            var days = new List<DateTime>();
            var currentDate = CurrentSprint.StartDate.Date;
            var endDate = CurrentSprint.FinishDate.Date;

            while (currentDate <= endDate)
            {
                days.Add(currentDate);
                currentDate = currentDate.AddDays(1);
            }
            return days;
        }

        private List<DatePoint> GetCompletedTasksByDay(List<DateTime> sprintDays)
        {
            var result = new List<DatePoint>();
            var completedTasks = new Dictionary<DateTime, int>();

            var taskLogs = CurrentSprint.Tasks
                .SelectMany(t => MockTaskMovementLogs.GetTaskMovementLogsByTaskId(t.Id))
                .Where(log => log.Status is Models.Projects.Enums.TaskStatus.Done);

            foreach (var day in sprintDays)
            {
                var completedOnDay = taskLogs.Count(log => log.StartDate.Date == day.Date);

                completedTasks[day] = completedOnDay;
            }

            var cumulativeCompleted = 0;
            foreach (var day in sprintDays)
            {
                cumulativeCompleted += completedTasks.GetValueOrDefault(day, 0);
                result.Add(new DatePoint
                {
                    Date = day,
                    Value = cumulativeCompleted
                });
            }

            return result;
        }

        private List<DatePoint> GetRemainingTasksByDay(List<DateTime> sprintDays)
        {
            var result = new List<DatePoint>();
            var totalTasks = CurrentSprint.Tasks.Count;

            foreach (var day in sprintDays)
            {
                var remainingTasks = CurrentSprint.Tasks.Count(task =>
                {
                    var statusOnDay = GetTaskStatusOnDate(task, day);
                    return statusOnDay != Models.Projects.Enums.TaskStatus.Done;
                });

                result.Add(new DatePoint
                {
                    Date = day,
                    Value = remainingTasks
                });
            }

            return result;
        }

        private Models.Projects.Enums.TaskStatus GetTaskStatusOnDate(Models.Projects.Entities.Task task, DateTime date)
        {
            var logs = MockTaskMovementLogs.GetTaskMovementLogsByTaskId(task.Id);
            if (logs.Count == 0) return task.Status;

            var activeLog = logs
                .Where(log => log.StartDate.Date <= date.Date)
                .Where(log => !log.EndDate.HasValue || log.EndDate.Value.Date >= date.Date)
                .OrderByDescending(log => log.StartDate)
                .FirstOrDefault();

            return activeLog?.Status ?? task.Status;
        }

        private List<DatePoint> GetIdealBurndown(List<DateTime> sprintDays)
        {
            var totalTasks = CurrentSprint.Tasks.Count;

            return [.. sprintDays.Select((day, index) => new DatePoint
            {
                Date = day,
                Value = totalTasks - (totalTasks * index / Math.Max(sprintDays.Count - 1, 1))
            })];
        }

        private ApexChartOptions<DatePoint> GetSprintBurndownOptions() => new()
        {
            Chart = new Chart
            {
                Type = ChartType.Area,
                Stacked = false,
                Toolbar = new Toolbar { Show = false }
            },
            Yaxis =
            [
                new YAxis
                {
                    Title = new AxisTitle { Text = "Количество задач" },
                    Min = 0,
                    Max = CurrentSprint.Tasks.Count + 2,
                    TickAmount = CurrentSprint.Tasks.Count + 2
                }
            ],
            Xaxis = new XAxis
            {
                Type = XAxisType.Datetime,
                Labels = new XAxisLabels
                {
                    Format = "dd.MM",
                    DatetimeUTC = false,
                    HideOverlappingLabels = true,
                    Rotate = -45
                },
                TickPlacement = TickPlacement.On
            },
            Stroke = new Stroke
            {
                Curve = Curve.Straight,
                Width = new List<double> { 2, 2, 2 }
            },
            Fill = new Fill
            {
                Type = new List<FillType> { FillType.Solid, FillType.Solid, FillType.Solid },
                Opacity = new List<double> { 0.1, 0.2, 0.3 }
            },
            DataLabels = new DataLabels
            {
                Enabled = false
            },
            Legend = new Legend
            {
                Position = LegendPosition.Top,
                HorizontalAlign = Align.Left
            },
            Annotations = new Annotations
            {
                Xaxis = [.. GetSprintDays()
                    .Where(day => day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                    .Select(weekend => new AnnotationsXAxis
                    {
                        X = weekend.ToOADate(),
                        BorderColor = "#ffeb3b",
                        Opacity = 0.1,
                        Label = new Label
                        {
                            Text = weekend.DayOfWeek == DayOfWeek.Saturday ? "Сб" : "Вс",
                            Style = new Style { Background = "#fff3cd" }
                        }
                    })]
            }
        };
    }
}
