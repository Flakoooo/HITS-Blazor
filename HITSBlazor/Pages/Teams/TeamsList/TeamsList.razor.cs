using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Skills;
using HITSBlazor.Services.Teams;
using HITSBlazor.Services.UserSkills;
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
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private IUserSkillService UserSkillService { get; set; } = null!;

        [Inject]
        private ITeamService TeamService { get; set; } = null!;

        [Inject]
        private ISkillService SkillService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public Guid? TeamId { get; set; }

        private bool _isLoading = true;

        private List<Team> _teams = [];
        private List<Skill> _skills = [];
        private List<Skill> _userSkills = [];

        private string? _searchTeamText = null;
        private string? _searchSkillText = null;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            await LoadTeamsAsync();
            await LoadSkillsAsync();
            if (AuthService.CurrentUser is not null)
                _userSkills = await UserSkillService.GetUserSkillsAsync(AuthService.CurrentUser.Id);

            _isLoading = false;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (TeamId is not null)
                ShowTeam((Guid)TeamId);
        }

        private async Task LoadTeamsAsync()
        {
            _teams = await TeamService.GetTeamsAsync(
                searchText: _searchTeamText
            );
            StateHasChanged();
        }

        private async Task LoadSkillsAsync()
        {
            _skills = await SkillService.GetSkillsAsync(
                searchText: _searchSkillText
            );
            StateHasChanged();
        }

        private async Task SearchTeam(string value)
        {
            _searchTeamText = value;
            await LoadTeamsAsync();
        }

        private async Task SearchSkill(string value)
        {
            _searchSkillText = value;
            await LoadSkillsAsync();
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
