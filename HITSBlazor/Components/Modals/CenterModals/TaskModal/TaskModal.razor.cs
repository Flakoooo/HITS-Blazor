using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Utils.Mocks.Common;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.CenterModals.TaskModal
{
    public partial class TaskModal
    {
        [Parameter]
        public Models.Projects.Entities.Task? CurrentTask { get; set; }

        private string TaskName { get; set; } = string.Empty;
        private string TaskDescription { get; set; } = string.Empty;
        private int Hours { get; set; }

        private List<Tag> _tags = [];
        private HashSet<Tag> SelectedTags { get; set; } = [];

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            if (CurrentTask is not null)
            {
                TaskName = CurrentTask.Name;
                TaskDescription = CurrentTask.Description;
                Hours = CurrentTask.WorkHour;
            }

            _tags = MockTags.GetTags();
        }
    }
}
