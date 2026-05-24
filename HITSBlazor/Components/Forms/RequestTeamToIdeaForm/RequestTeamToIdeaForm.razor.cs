using HITSBlazor.Components.Button;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Forms.RequestTeamToIdeaForm
{
    public partial class RequestTeamToIdeaForm
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private ITeamService TeamService { get; set; } = null!;

        [Parameter]
        public required IdeaMarket IdeaMarket { get; set; }

        [Parameter]
        public bool ShowSkillsCheckbox { get; set; } = false;

        [Parameter]
        public List<Skill> SelectedSkills { get; set; } = [];

        [Parameter]
        public EventCallback<List<Skill>> SelectedSkillsChanged { get; set; }

        private bool _isLoading = true;
        private bool _sumbitting = false;

        private Dictionary<string, string> _errors = [];

        private Func<Task>? _queuedCollapseMethod;

        private readonly List<Team> _teams = [];
        private Dictionary<Guid, RequestTeamToIdea> _cachedRequests = [];

        private readonly string _hintText = "В письме необходимо пояснить, почему именно ваша команда должна быть принята для реализации данной идеи." +
            " Основная задача мотивационного письма — выделить команду на фоне других претендентов и продемонстрировать максимальное соответствие требованиям." +
            " При подаче заявки в идею важно акцентировать внимание на определённых характеристиках и достижениях команды." +
            " С помощью письма инициатор идеи оценивает  — компетенции, опыт и достижения, которые есть у команды на момент заявления.";

        private string LetterText { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (AuthService.CurrentUser is not null)
            {
                TeamService.OnRequestTeamInIdeaStatusUpdated += EventedRequestUpdate;

                await LoadTeamsAsync();
                await LoadMoreCachedRequest();

                _isLoading = false;
                MarkAsInitialized();
            }
        }

        protected override async Task OnLoadMoreItemsAsync() => await LoadTeamsAsync(true);

        protected override int GetCurrentItemsCount() => _teams.Count;

        private async Task LoadTeamsAsync(bool append = false)
        {
            var currentUser = AuthService.CurrentUser;
            if (currentUser is null) return;

            await LoadDataAsync(
                _teams,
                () => TeamService.GetTeamsAsync(
                    _currentPage,
                    userId: currentUser.Id
                ),
                append
            );

            await LoadMoreCachedRequest();
        }

        private async Task LoadMoreCachedRequest()
        {
            var idsForLoad = _teams.Select(t => t.Id).Where(id => !_cachedRequests.ContainsKey(id));
            _cachedRequests = (await TeamService.GetTeamRequestsForCurretnIdeaMarketAndTeamsAsync(IdeaMarket.Id, idsForLoad))
                    .ToDictionary(r => r.TeamId, r => r);
        }

        private static string GetStatusButtonText(TeamRequestStatus? status) => status switch
        {
            TeamRequestStatus.Canceled => "Заявка отклонена",
            TeamRequestStatus.Annulled => "Команда в работе",
            _ => "Подать заявку"
        };

        private static ButtonVariant GetStatusButtonVariant(TeamRequestStatus? status) => status switch
        {
            TeamRequestStatus.Canceled => ButtonVariant.Danger,
            TeamRequestStatus.Annulled => ButtonVariant.Secondary,
            _ => ButtonVariant.Primary
        };

        private void WithdrawnRequest(Team team) => ModalService.ShowConfirmModal(
            $"Вы действительно хотите отозвать заявку команды {team.Name}?",
            () => TeamService.UpdateRequestTeamToIdeaStatusAsync(_cachedRequests[team.Id].Id, TeamRequestStatus.Withdrawn),
            confirmButtonVariant: ButtonVariant.Danger,
            confirmButtonText: "Отклонить заявку"
        );

        private async Task CheckTeamSkills(List<Skill> skills, bool isChecked)
        {
            SelectedSkills = isChecked ? skills : [];

            await SelectedSkillsChanged.InvokeAsync(SelectedSkills);
        }

        private async Task SendNewRequest(Team team)
        {
            if (_sumbitting) return;

            _sumbitting = true;
            _errors.Clear();

            if (string.IsNullOrWhiteSpace(LetterText))
                _errors.Add("letter", "Поле не может быть пустым");

            if (_errors.Count > 0) return;

            var newRequest = await TeamService.CreateRequestTeamToIdeaAsync(IdeaMarket, team, LetterText);
            _cachedRequests.Add(team.Id, newRequest);
            if (_queuedCollapseMethod is not null)
            {
                await _queuedCollapseMethod();
                _queuedCollapseMethod = null;
            }

            _sumbitting = false;
            StateHasChanged();
        }

        private async Task EventedRequestUpdate(Guid requestId, TeamRequestStatus newStatus)
        {
            foreach (var request in _cachedRequests)
                if (request.Value.Id == requestId)
                    request.Value.Status = newStatus;
            
            StateHasChanged();
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            TeamService.OnRequestTeamInIdeaStatusUpdated -= EventedRequestUpdate;

            await ValueTask.CompletedTask;
        }
    }
}
