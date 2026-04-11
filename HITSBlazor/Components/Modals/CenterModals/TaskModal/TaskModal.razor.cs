using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Services.Tags;
using HITSBlazor.Utils.Mocks.Common;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.CenterModals.TaskModal
{
    public partial class TaskModal
    {
        [Inject]
        private ITagService TagService { get; set; } = null!;

        [Parameter]
        public Models.Projects.Entities.Task? CurrentTask { get; set; }

        private bool _isLoading = true;

        private string TaskName { get; set; } = string.Empty;
        private string TaskDescription { get; set; } = string.Empty;
        private int? Hours { get; set; }

        private List<Tag> _tags = [];
        private HashSet<Tag> SelectedTags { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            _tags = await TagService.GetTagsAsync();

            if (CurrentTask is not null)
            {
                TaskName = CurrentTask.Name;
                TaskDescription = CurrentTask.Description;
                Hours = CurrentTask.WorkHour;
                SelectedTags = [.. CurrentTask.Tags];
            }

            _isLoading = false;
        }
    }
}
