using ApexCharts;
using HITSBlazor.Components.Modals.Components.RightSideModaCollapselInfo;
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

        private Models.Projects.Entities.Task? _selectedTask;

        private EndedSprintModalStatCategory _activeStatCategory = EndedSprintModalStatCategory.GeneralStat;
        private EndedSprintModalInfoCategory _activeInfoCategory = EndedSprintModalInfoCategory.Scores;

        private List<CollapseItem> _sprintData = [];

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
                if (currentDate.DayOfWeek != DayOfWeek.Saturday &&
                    currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    days.Add(currentDate);
                }
                currentDate = currentDate.AddDays(1);
            }
            return days;
        }

        private List<DatePoint> GetCompletedTasksByDay(List<DateTime> sprintDays)
        {
            var completedTasks = CurrentSprint.Tasks
                .Where(t => t.Status == Models.Projects.Enums.TaskStatus.InBackLog &&
                           t.FinishDate.HasValue)
                .GroupBy(t => t.FinishDate.Value.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            return sprintDays.Select(day => new DatePoint
            {
                Date = day,
                Value = completedTasks.GetValueOrDefault(day, 0)
            }).ToList();
        }

        private List<DatePoint> GetRemainingTasksByDay(List<DateTime> sprintDays)
        {
            var totalTasks = CurrentSprint.Tasks.Count;
            var result = new List<DatePoint>();

            foreach (var day in sprintDays)
            {
                var completedToday = CurrentSprint.Tasks
                    .Count(t => t.Status == Models.Projects.Enums.TaskStatus.InBackLog &&
                               t.FinishDate.HasValue &&
                               t.FinishDate.Value.Date <= day);

                result.Add(new DatePoint
                {
                    Date = day,
                    Value = totalTasks - completedToday
                });
            }

            return result;
        }

        private List<DatePoint> GetIdealBurndown(List<DateTime> sprintDays)
        {
            var totalTasks = CurrentSprint.Tasks.Count;
            var workingDays = sprintDays.Count;

            return sprintDays.Select((day, index) => new DatePoint
            {
                Date = day,
                Value = totalTasks - (totalTasks * index / (workingDays - 1))
            }).ToList();
        }

        private ApexChartOptions<Models.Projects.Entities.Task> GetSprintBurndownOptions() => new()
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
                    Max = CurrentSprint.Tasks.Count + 2
                }
            ],
            Xaxis = new XAxis
            {
                Type = XAxisType.Datetime,
                Labels = new XAxisLabels
                {
                    Format = "dd.MM",
                    DatetimeUTC = false
                }
            },
            Stroke = new Stroke
            {
                Curve = Curve.Smooth,
                Width = new List<double> { 2, 3, 0 }
            },
            Fill = new Fill
            {
                Type = new List<FillType> { FillType.Solid, FillType.Gradient, FillType.Solid }
            },
            Tooltip = new Tooltip
            {
                Shared = true,
                Intersect = false,
                X = new TooltipX { Format = "dd.MM.yyyy" }
            },
            Legend = new Legend
            {
                Position = LegendPosition.Top,
                HorizontalAlign = Align.Left
            }
        };
    }
}
