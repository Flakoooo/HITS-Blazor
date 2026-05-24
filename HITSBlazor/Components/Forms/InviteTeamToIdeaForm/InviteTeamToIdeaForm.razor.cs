using HITSBlazor.Components.Button;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.IdeaMarkets;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Forms.InviteTeamToIdeaForm
{
    public partial class InviteTeamToIdeaForm
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private ITeamService TeamService { get; set; } = null!;

        [Inject]
        private IIdeaMarketService IdeaMarketService { get; set; } = null!;

        [Parameter]
        public required Team CurrentTeam { get; set; }

        private bool _isLoading = true;
        private bool _sumbitting = false;

        private Func<Task>? _queuedCollapseMethod;

        private readonly List<IdeaMarket> _ideaMarkets = [];
        private Dictionary<Guid, InvitationTeamToIdea> _cachedInvites = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (AuthService.CurrentUser is not null)
            {
                TeamService.OnInvitationTeamInIdeaStatusUpdated += EventedRequestUpdate;

                await LoadIdeaMarketAsync();
                await LoadMoreCachedInvites();

                _isLoading = false;
                MarkAsInitialized();
            }
        }

        protected override async Task OnLoadMoreItemsAsync() => await LoadIdeaMarketAsync(true);

        protected override int GetCurrentItemsCount() => _ideaMarkets.Count;

        private async Task LoadIdeaMarketAsync(bool append = false)
        {
            await LoadDataAsync(
                _ideaMarkets,
                () => IdeaMarketService.GetIdeasMarketAsync(
                    _currentPage
                ),
                append
            );
        }

        private async Task LoadMoreCachedInvites()
        {
            var idsForLoad = _ideaMarkets.Select(im => im.Id).Where(id => !_cachedInvites.ContainsKey(id));
            _cachedInvites = (await TeamService.GetTeamInvitationForCurrentTeamAndIdeaMarketsAsync(CurrentTeam.Id, idsForLoad))
                    .ToDictionary(r => r.IdeaId, r => r);
        }

        private static string GetStatusButtonText(TeamRequestStatus? status) => status switch
        {
            TeamRequestStatus.Canceled => "Приглашение отклонено",
            TeamRequestStatus.Annulled => "Команда в работе",
            _ => "Отправить приглашение"
        };

        private static ButtonVariant GetStatusButtonVariant(TeamRequestStatus? status) => status switch
        {
            TeamRequestStatus.Canceled => ButtonVariant.Danger,
            TeamRequestStatus.Annulled => ButtonVariant.Secondary,
            _ => ButtonVariant.Primary
        };

        private void WithdrawnInvitation(IdeaMarket ideaMarket) => ModalService.ShowConfirmModal(
            $"Вы действительно хотите отозвать приглашение команды в идею {ideaMarket.Name}?",
            () => TeamService.UpdateRequestTeamToIdeaStatusAsync(_cachedInvites[ideaMarket.Id].Id, TeamRequestStatus.Withdrawn),
            confirmButtonVariant: ButtonVariant.Danger,
            confirmButtonText: "Отозвать приглашение"
        );

        private async Task SendNewInvite(IdeaMarket ideaMarket)
        {
            if (_sumbitting) return;

            _sumbitting = true;

            var newInvitation = await TeamService.CreateInvitationTeamToIdeaAsync(ideaMarket, CurrentTeam);
            if (newInvitation is not null)
            {
                _cachedInvites.Add(ideaMarket.Id, newInvitation);
                if (_queuedCollapseMethod is not null)
                {
                    await _queuedCollapseMethod();
                    _queuedCollapseMethod = null;
                }
            }

            _sumbitting = false;
            StateHasChanged();
        }

        private async Task EventedRequestUpdate(Guid requestId, TeamRequestStatus newStatus)
        {
            foreach (var request in _cachedInvites)
                if (request.Value.Id == requestId)
                    request.Value.Status = newStatus;
            
            StateHasChanged();
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            TeamService.OnInvitationTeamInIdeaStatusUpdated -= EventedRequestUpdate;

            await ValueTask.CompletedTask;
        }
    }
}
