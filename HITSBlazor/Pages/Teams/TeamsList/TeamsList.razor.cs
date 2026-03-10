using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Tables.TableHeader;
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

        private static readonly List<TableHeaderItem> _teamTableHeader =
        [
            new() { Text = "Приватность",   InCentered = true,  OrderBy = nameof(Team.Closed)           },
            new() { Text = "Название",  ColumnClass = "col-3",  OrderBy = nameof(Team.Name)             },
            new() { Text = "Статус",        InCentered = true,  OrderBy = nameof(Team.HasActiveProject) },
            new() { Text = "Участники",     InCentered = true,  OrderBy = nameof(Team.MembersCount)     },
            new() { Text = "Дата создания", InCentered = true,  OrderBy = nameof(Team.CreatedAt)        }
        ];

        private bool _isLoading = true;

        private List<Team> _teams = [];
        private List<Skill> _skills = [];
        private HashSet<Guid> _userSkillIds = [];
        private HashSet<Guid> _selectedSkillIds = [];

        private string? _searchText = null;
        private string? _orderTeamBy = null;
        private bool? _sortTeamState = null;
        private bool? _privacyState = null;
        private bool? _surveyState = null;
        private bool? _hasActiveProjectState = null;
        private bool? _skillsState = null;
        private string? _searchSkillText = null;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            await LoadTeamsAsync();
            if (AuthService.CurrentUser is not null)
                _userSkillIds = [.. (await UserSkillService.GetUserSkillsAsync(AuthService.CurrentUser.Id)).Select(s => s.Id)];

            await LoadSkillsAsync();

            _isLoading = false;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (TeamId is not null)
                ShowTeam((Guid)TeamId);
        }

        private async Task LoadTeamsAsync()
        {
            var filter = new TeamsFilter(
                SearchText: _searchText,
                Privacy: _privacyState,
                Survey: _surveyState,
                HasActiveProject: _hasActiveProjectState,
                SearchSkillIds: _selectedSkillIds,
                OrderBy: _orderTeamBy,
                ByDescending: _sortTeamState
            );
            _teams = await TeamService.GetTeamsAsync(filter);
            StateHasChanged();
        }

        private async Task LoadSkillsAsync()
        {
            _skills = await SkillService.GetSkillsAsync(searchText: _searchSkillText);

            _skills = [.. _skills
                .OrderByDescending(s => _userSkillIds.Contains(s.Id))
                .ThenBy(s => s.Name)];
            StateHasChanged();
        }

        private async Task SearchTeam(string value)
        {
            _searchText = value;
            await LoadTeamsAsync();
        }

        private async Task SearchSkill(ChangeEventArgs e)
        {
            _searchSkillText = e.Value?.ToString();
            await LoadSkillsAsync();
        }

        private async Task SelectedSkillsChanged(Guid skillId, bool isChecked)
        {
            if (isChecked) _selectedSkillIds.Add(skillId);
            else           _selectedSkillIds.Remove(skillId);

            await LoadTeamsAsync();
        }

        private async Task SortTeam(string? value, bool? state)
        {
            _orderTeamBy = value;
            _sortTeamState = state;
            await LoadTeamsAsync();
        }

        private void ShowTeam(Guid teamId) => ModalService.ShowTeamModal(teamId);

        //TODO: сброс фильтров не влияет на иконки фильтров в хэдэре таблицы
        private async Task ResetFilters()
        {
            _sortTeamState = null;
            _privacyState = null;
            _surveyState = null;
            _hasActiveProjectState = null;
            _skillsState = null;
            _searchSkillText = null;
            _selectedSkillIds = [];

            await LoadTeamsAsync();
        }

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
