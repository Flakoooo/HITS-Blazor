using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
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
        private ITeamService TeamService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public Guid? TeamId { get; set; }

        private List<Team> _teams = [];

        private string? _searchText = null;

        protected override async Task OnInitializedAsync()
        {
            await LoadTeamsAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (TeamId is not null)
                ShowTeam((Guid)TeamId);
        }

        private async Task LoadTeamsAsync()
        {
            _teams = await TeamService.GetTeamsAsync(
                searchText: _searchText
            );
            StateHasChanged();
        }

        private async Task SearchIdea(string value)
        {
            _searchText = value;
            await LoadTeamsAsync();
        }

        private void ShowTeam(Guid teamId) => ModalService.ShowTeamModal(teamId);

        private async Task OnTeamAction(TableActionContext context)
        {
            if (context.Action == MenuAction.View)
            {
                if (context.Item is Guid guid)
                    ShowTeam(guid);
            }
            else if (context.Action == MenuAction.Edit)
            {
                if (context.Item is Guid guid)
                    await NavigationService.NavigateToAsync($"/teams/create/{guid}");
            }
            else if (context.Action == MenuAction.Delete)
            {
                if (context.Item is not Team team || !await TeamService.DeleteTeamAsync(team))
                    return;

                _teams.Remove(team);
            }
        }
    }
}
