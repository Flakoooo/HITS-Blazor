using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using HITSBlazor.Utils.Models;
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
        public string TeamId { get; set; } = string.Empty;

        private static readonly List<TableHeaderItem> _teamTableHeader =
        [
            new() { Text = "Приватность",   InCentered = true,  OrderBy = nameof(Team.Closed)           },
            new() { Text = "Название",  ColumnClass = "col-3",  OrderBy = nameof(Team.Name)             },
            new() { Text = "Статус",        InCentered = true,  OrderBy = nameof(Team.HasActiveProject) },
            new() { Text = "Участники",     InCentered = true,  OrderBy = nameof(Team.MembersCount)     },
            new() { Text = "Дата создания", InCentered = true,  OrderBy = nameof(Team.CreatedAt)        }
        ];

        private bool _isLoading = true;

        private readonly List<Team> _teams = [];

        private string? _searchText = null;
        private string? _orderTeamBy = null;
        private bool? _sortTeamState = null;

        private readonly List<ValueViewModel<bool?>> _isClosedFilterValues =
        [
            new(true, "Закрытая команда"),
            new(false, "Открытая команда")
        ];
        private ValueViewModel<bool?>? IsClosed { get; set; }

        private readonly List<ValueViewModel<bool?>> _hasActiveProjectFilterValues =
        [
            new(true, "В работе"),
            new(false, "В поисках")
        ];
        private ValueViewModel<bool?>? HasActiveProjectState { get; set; }

        private readonly List<ValueViewModel<bool?>> _skillProjectFilterValues =
        [
            new(true, "Искать везде"),
            new(false, "Искать по вакансиямх")
        ];
        private ValueViewModel<bool?>? SkillsState { get; set; }

        private string SeacrhSkillText { get; set; } = string.Empty;
        private HashSet<Guid> SelectedSkillIds { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            await LoadTeamsAsync();

            _isLoading = false;

            MarkAsInitialized();
        }

        protected override void OnParametersSet()
        {
            if (!string.IsNullOrWhiteSpace(TeamId) && Guid.TryParse(TeamId, out Guid teamId))
                ModalService.ShowTeamModal(teamId);
        }

        protected override int GetCurrentItemsCount() => _teams.Count;

        protected override async Task OnLoadMoreItemsAsync()
        {
            await LoadTeamsAsync(append: true);
        }

        private async Task LoadTeamsAsync(bool append = false)
        {
            if (!append)
            {
                ResetPagination();
                _teams.Clear();
            }

            StateHasChanged();

            var listResponse = await TeamService.GetTeamsAsync(
                _currentPage,
                searchText: _searchText,
                privacy: IsClosed?.Value,
                hasActiveProject: HasActiveProjectState?.Value,
                searchSkillIds: SelectedSkillIds,
                orderBy: _orderTeamBy,
                byDescending: _sortTeamState
            );

            _totalCount = listResponse.Count;
            if (listResponse.List.Count > 0)
            {
                if (append)
                    _teams.AddRange(listResponse.List);
                else
                {
                    _teams.Clear();
                    _teams.AddRange(listResponse.List);
                }

                IncrementPage();
            }

            StateHasChanged();
        }

        private async Task SearchTeam(string value)
        {
            _searchText = value;
            ResetPagination();
            await LoadTeamsAsync();
        }

        private async Task SortTeam(string? value, bool? state)
        {
            _orderTeamBy = value;
            _sortTeamState = state;
            ResetPagination();
            await LoadTeamsAsync();
        }

        private async Task ShowTeam(Guid teamId)
        {
            if (AuthService.CurrentUser?.Role is RoleType.Admin)
                await NavigationService.NavigateToAsync($"/teams/list/{teamId}");
            else
                ModalService.ShowIdeaModal(teamId);
        }

        private async Task ResetFilters()
        {
            _sortTeamState = null;
            IsClosed = null;
            HasActiveProjectState = null;
            SkillsState = null;
            SeacrhSkillText = string.Empty;
            SelectedSkillIds = [];

            foreach (var header in _teamTableHeader) 
                header.IsOrdered = null;

            ResetPagination();

            await LoadTeamsAsync();
        }

        private async Task OnTeamAction(TableActionContext context)
        {
            if (context.Item is Guid teamId)
            {
                if (context.Action is MenuAction.View) 
                    await ShowTeam(teamId);
                else if (context.Action is MenuAction.Edit)
                    await NavigationService.NavigateToAsync($"/teams/create/{teamId}");
            }
            else if (context.Action is MenuAction.Delete)
            {
                if (context.Item is not Team team) return;

                ModalService.ShowConfirmModal(
                    $"Вы действительно хотите удалить {team.Name}?",
                    () => TeamService.DeleteTeamAsync(team),
                    confirmButtonVariant: ButtonVariant.Danger,
                    confirmButtonText: "Удалить"
                );
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await ValueTask.CompletedTask;
        }
    }
}
