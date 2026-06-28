using ApexCharts;
using HITSBlazor.Components.Modals.Components.RightSideModaCollapselInfo;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Services.Projects;
using HITSBlazor.Services.Tags;
using HITSBlazor.Utils.Mocks.Projects;
using Microsoft.AspNetCore.Components;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;
using SharpTask = System.Threading.Tasks.Task;

namespace HITSBlazor.Components.Modals.CenterModals.EndedSprintModal
{
    public partial class EndedSprintModal
    {
        [Inject]
        private IProjectService ProjectService { get; set; } = null!;


        [Inject]
        private ITagService TagService { get; set; } = null!;

        [Parameter]
        public required Guid ProjectId { get; set; }

        [Parameter]
        public required Sprint CurrentSprint { get; set; }

        private bool _isLoading = true;
        private string _seacrhText = string.Empty;

        private readonly List<HITSTask> _tasks = [];

        private List<SprintMarks> _sprintMarks = [];

        private HashSet<Tag> SelectedTags { get; set; } = [];
        private SprintMarks? SelectedMember { get; set; }

        private EndedSprintModalStatCategory _activeStatCategory = EndedSprintModalStatCategory.GeneralStat;
        private EndedSprintModalInfoCategory _activeInfoCategory = EndedSprintModalInfoCategory.Scores;

        private List<CollapseItem> _sprintData = [];

        private HITSTask? _selectedTask;
        private List<CollapseItem> _taskData = [];

        private ApexChartOptions<DatePoint> _taskApexChart = new();

        protected override async SharpTask OnInitializedAsync()
        {
            _isLoading = true;

            _sprintMarks = await ProjectService.GetSprintMarksBySprintIdAsync(CurrentSprint.Id);

            await LoadTasksAsync();

            _sprintData = 
            [
                new() { Title = "Описание", Data = CurrentSprint.Goal },
                new() { Title = "Отчет",    Data = CurrentSprint.Report },
            ];

            _taskApexChart = GetSprintBurndownOptions();

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async SharpTask OnLoadMoreItemsAsync() => await LoadTasksAsync(append: true);

        protected override int GetCurrentItemsCount() => _tasks.Count;

        private async SharpTask LoadTasksAsync(bool append = false)
        {
            await LoadDataAsync(
                _tasks,
                () => ProjectService.GetTasksByQueryParamsAsync(
                    _currentPage,
                    sprintId: CurrentSprint.Id,
                    searchText: _seacrhText,
                    selectedTags: SelectedTags.Select(t => t.Id),
                    selectedExecutors: SelectedMember?.UserId is not null ? [SelectedMember.UserId] : null
                ),
                append
            );
        }

        private async SharpTask FiltersHasChanged()
        {
            ResetPagination();
            await LoadTasksAsync();
        }

        private async SharpTask SeacrhTask(string value)
        {
            _seacrhText = value;
            await FiltersHasChanged();
        }

        private async SharpTask SelectTag(HashSet<Tag> newTags)
        {
            SelectedTags = newTags;
            await FiltersHasChanged();
        }

        private async SharpTask SelectMember(SprintMarks? selectedMember)
        {
            SelectedMember = selectedMember;
            await FiltersHasChanged();
        }

        private void SelectTask(HITSTask task)
        {
            if (_selectedTask?.Id == task.Id) return;

            _selectedTask = task;
            _taskData = [new() { Title = "Описание", Data = task.Description }];

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

        private static string GetInfoCategoryName(EndedSprintModalInfoCategory category) => category switch
        {
            EndedSprintModalInfoCategory.Scores => "Оценки",
            EndedSprintModalInfoCategory.Info => "Информация",
            _ => category.ToString()
        };

        private async System.Threading.Tasks.Task ChangeCategory(EndedSprintModalStatCategory category)
        {
            _activeStatCategory = category;
        }

        private List<DateOnly> GetSprintDays()
        {
            var days = new List<DateOnly>();
            var currentDate = CurrentSprint.StartDate;
            var endDate = CurrentSprint.FinishDate;

            while (currentDate <= endDate)
            {
                days.Add(currentDate);
                currentDate = currentDate.AddDays(1);
            }
            return days;
        }

        private List<DatePoint> GetCompletedTasksByDay(List<DateOnly> sprintDays)
        {
            var result = new List<DatePoint>();
            var completedTasks = new Dictionary<DateOnly, int>();

            var taskLogs = CurrentSprint.Tasks
                .SelectMany(t => MockTaskMovementLogs.GetTaskMovementLogsByTaskId(t.Id))
                .Where(log => log.Status is Models.Projects.Enums.TaskStatus.Done);

            foreach (var day in sprintDays)
            {
                var completedOnDay = taskLogs.Count(log => DateOnly.FromDateTime(log.StartDate) == day);

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

        private List<DatePoint> GetRemainingTasksByDay(List<DateOnly> sprintDays)
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

        private Models.Projects.Enums.TaskStatus GetTaskStatusOnDate(Models.Projects.Entities.Task task, DateOnly date)
        {
            var logs = MockTaskMovementLogs.GetTaskMovementLogsByTaskId(task.Id);
            if (logs.Count == 0) return task.Status;

            var activeLog = logs
                .Where(log => DateOnly.FromDateTime(log.StartDate) <= date)
                .Where(log => !log.EndDate.HasValue || DateOnly.FromDateTime(log.EndDate.Value) >= date)
                .OrderByDescending(log => log.StartDate)
                .FirstOrDefault();

            return activeLog?.Status ?? task.Status;
        }

        private List<DatePoint> GetIdealBurndown(List<DateOnly> sprintDays)
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
                        X = weekend.ToDateTime(TimeOnly.MinValue).ToOADate(),
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
