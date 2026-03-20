using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
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
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private ITeamService TeamService { get; set; } = null!;

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

        private string? _searchText = null;
        private string? _orderTeamBy = null;
        private bool? _sortTeamState = null;
        private bool? PrivacyState { get; set; } = null;
        private bool? SurveyState { get; set; } = null;
        private bool? HasActiveProjectState { get; set; } = null;
        private bool? SkillsState { get; set; } = null;

        private string SeacrhSkillText { get; set; } = string.Empty;
        private HashSet<Guid> SelectedSkillIds { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            await LoadTeamsAsync();

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
                Privacy: PrivacyState,
                Survey: SurveyState,
                HasActiveProject: HasActiveProjectState,
                SearchSkillIds: SelectedSkillIds,
                OrderBy: _orderTeamBy,
                ByDescending: _sortTeamState
            );
            _teams = await TeamService.GetTeamsAsync(filter);
            StateHasChanged();
        }

        private async Task SearchTeam(string value)
        {
            _searchText = value;
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
            PrivacyState = null;
            SurveyState = null;
            HasActiveProjectState = null;
            SkillsState = null;
            SeacrhSkillText = string.Empty;
            SelectedSkillIds = [];

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
