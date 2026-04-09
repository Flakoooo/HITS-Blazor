using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Components.Modals.CenterModals.TaskModal
{
    public partial class TaskModal
    {
        private string TaskName { get; set; } = string.Empty;
        private string TaskDescription { get; set; } = string.Empty;
        private string Hours { get; set; } = string.Empty;

        private List<Tag> _tags = [];
        private HashSet<Tag> SelectedTags { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _tags = MockTags.GetTags();
        }
    }
}
