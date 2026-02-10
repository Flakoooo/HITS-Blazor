using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Teams.TeamsList
{
    [Authorize]
    [Route("/teams/list")]
    [Route("/teams/list/{TeamId}")]
    public partial class TeamsList
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public Guid? TeamId { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (TeamId is not null)
                ShowTeam((Guid)TeamId);
        }

        private void ShowTeam(Guid teamId) => ModalService.ShowTeamModal(teamId);
    }
}
