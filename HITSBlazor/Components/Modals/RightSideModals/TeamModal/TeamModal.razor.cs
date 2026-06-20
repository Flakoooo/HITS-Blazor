using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.Components.RightSideModaCollapselInfo;
using HITSBlazor.Components.Modals.Components.RightSideModalInfo;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.RightSideModals.TeamModal
{
    public partial class TeamModal : IDisposable
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
        private bool _sendRequestAllowed = false;

        private List<CollapseItem> _teamData = [];

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

            TeamService.OnTeamLeaderHasChanged += TeamLeaderHasChanged;
            TeamService.OnRequestToTeamStatusHasChanged += TeamRequestsStatusChanged;
            TeamService.OnTeamInvitationStatusHasChanged += TeamRequestsStatusChanged;

            _sendRequestAllowed = await TeamService.CurrentUserCanSendRequestInTeamAsync(_currentTeam.Id);

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

        private void TeamLeaderHasChanged(Guid teamId, TeamMember? newLeader)
        {
            if (_currentTeam is not null && _currentTeam.Id == teamId)
            {
                _currentTeam.Leader = newLeader;
                var leaderInfoItem = _infoItems[1];
                leaderInfoItem.Text = _currentTeam.Leader?.FullName ?? "-";
                if (_currentTeam.Leader is not null)
                {
                    leaderInfoItem.IsLinkable = true;
                    leaderInfoItem.LinkMethod = () => ModalService.ShowProfileModal(_currentTeam.Leader.UserId);
                }
                StateHasChanged();
            }
        }

        private void TeamRequestsStatusChanged(Guid id, TeamRequestStatus newStatus)
        {
            if (_currentTeam is not null && newStatus is TeamRequestStatus.Accepted)
            {
                ++_currentTeam.MembersCount;
                _infoItems[3].Text = _currentTeam.MembersCount.ToString();
                StateHasChanged();
            }
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

            ModalService.ShowInviteUsersModal([], _currentTeam.Id);
        }

        private void SendRequestInTeam()
        {
            if (_currentTeam is null) return;

            ModalService.ShowConfirmModal(
                $"Вы действительно хотите подать заявку в {_currentTeam.Name}?",
                () => TeamService.CreateNewRequestToTeam(_currentTeam.Id),
                confirmButtonVariant: ButtonVariant.Success,
                confirmButtonText: "Подать"
            );
        }

        public void Dispose()
        {
            TeamService.OnTeamLeaderHasChanged -= TeamLeaderHasChanged;
            TeamService.OnRequestToTeamStatusHasChanged -= TeamRequestsStatusChanged;
            TeamService.OnTeamInvitationStatusHasChanged -= TeamRequestsStatusChanged;
        }
    }
}
