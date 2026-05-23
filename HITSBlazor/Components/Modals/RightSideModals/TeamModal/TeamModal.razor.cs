using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.Components.RightSideModaCollapselInfo;
using HITSBlazor.Components.Modals.Components.RightSideModalInfo;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.RightSideModals.TeamModal
{
    public partial class TeamModal
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private ITeamService TeamService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Parameter]
        public Guid TeamId { get; set; }

        private bool _isLoading = true;

        private Team? _currentTeam;

        private List<CollapseItem> _teamData = [];

        private string _searchText = string.Empty;

        private TeamTableCategory _activeTableCategory  = TeamTableCategory.Members;

        private readonly List<RightSideModalInfoItem> _infoItems =
        [
            new RightSideModalInfoItem
            {
                Label = "Владелец команды",
                Icon = "bi-person-circle",
                IsLinkable = true
            },
            new RightSideModalInfoItem
            {
                Label = "Тим-лидер команды",
                Icon = "bi-person-circle"
            },
            new RightSideModalInfoItem
            {
                Label = "Дата создания",
                Icon = "bi-calendar-date",
                TextColor = Typography.TextColor.Primary
            },
            new RightSideModalInfoItem
            {
                IsInline = true,
                Label = "Количество участников:",
                TextColor = Typography.TextColor.Primary
            }
        ];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            _currentTeam = await TeamService.GetTeamByIdAsync(TeamId);
            if (_currentTeam is null) return;

            _teamData = GetTeamData();

            var ownerInfoItem = _infoItems[0];
            ownerInfoItem.Text = _currentTeam.Owner.FullName;
            ownerInfoItem.LinkMethod = () => ModalService.ShowProfileModal(_currentTeam.Owner.UserId);

            var leaderInfoItem = _infoItems[1];
            leaderInfoItem.Text = _currentTeam.Leader?.FullName ?? "-";
            if (_currentTeam.Leader is not null)
            {
                leaderInfoItem.IsLinkable = true;
                leaderInfoItem.LinkMethod = () => ModalService.ShowProfileModal(_currentTeam.Leader.UserId);
            }

            _infoItems[2].Text = _currentTeam.CreatedAt.ToString("dd.MM.yyyy");

            _infoItems[3].Text = _currentTeam.MembersCount.ToString();

            _isLoading = false;
        }

        private List<CollapseItem> GetTeamData() => [
            new() { Title = "Описание команды", Data = _currentTeam?.Description },
        ];

        private async Task ChangeCategory(TeamTableCategory category)
        {
            _activeTableCategory = category;
            StateHasChanged();
        }

        private async Task NavigateToCreateTeam()
        {
            await ModalService.CloseAll(ModalType.RightSide);
            await NavigationService.NavigateToAsync($"/teams/create/{_currentTeam?.Id}");
        }

        private void DeleteTeam()
        {
            if (_currentTeam is null) return;

            ModalService.ShowConfirmModal(
                $"Вы действительно хотите удалить {_currentTeam.Name}?",
                () => TeamService.DeleteTeamAsync(_currentTeam),
                confirmButtonVariant: ButtonVariant.Danger,
                confirmButtonText: "Удалить"
            );
        }

        private void ShowInviteUsersModal()
        {
            if (_currentTeam is null) return;

            ModalService.ShowInviteUsersModal(
                _currentTeam.Members.Select(m => m.UserId).ToHashSet(),
                _currentTeam.Id
            );
        }
    }
}
