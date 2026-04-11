using HITSBlazor.Models.Projects.Entities;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.CenterModals.FinishSprintModal
{
    public partial class FinishSprintModal
    {
        [Parameter]
        public List<ProjectMember> ProjectMembers { get; set; } = [];

        private Dictionary<Guid, int?> _projectMembersScores = [];

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            _projectMembersScores = ProjectMembers.ToDictionary(m => m.UserId, m => (int?)null);
        }
    }
}
