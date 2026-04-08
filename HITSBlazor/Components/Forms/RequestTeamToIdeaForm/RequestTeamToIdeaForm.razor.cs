using HITSBlazor.Components.Button;
using HITSBlazor.Components.Collapse;
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
using System.Threading.Tasks;

namespace HITSBlazor.Components.Forms.RequestTeamToIdeaForm
{
    public partial class RequestTeamToIdeaForm : IDisposable
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private ITeamService TeamService { get; set; } = null!;

        [Inject]
        private IIdeaMarketService IdeaMarketService { get; set; } = null!;

        [Parameter]
        public required IdeaMarket IdeaMarket { get; set; }

        [Parameter]
        public bool ShowSkillsCheckbox { get; set; } = false;

        [Parameter]
        public List<Skill> SelectedSkills { get; set; } = [];

        [Parameter]
        public EventCallback<List<Skill>> SelectedSkillsChanged { get; set; }

        private bool _isLoading = true;
        private bool _sumbitted = false;
        private bool _sumbitting = false;

        private Func<Task>? _queuedCollapseMethod;

        private List<Team> _teams = [];
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
                _teams = await TeamService.GetTeamsByOwnerOrLeaderId(AuthService.CurrentUser.Id);
                var requestsTeamToIdea = await IdeaMarketService.GetRequestsTeamToIdeaAsync(IdeaMarket.Id);

                _cachedRequests = (await IdeaMarketService.GetRequestsTeamToIdeaAsync(IdeaMarket.Id))
                    .Where(r => _teams.Select(t => t.Id).Contains(r.TeamId))
                    .ToDictionary(r => r.TeamId, r => r);

                TeamService.OnRequestsStatusUpdated += EventedRequestUpdate;

                _isLoading = false;
            }
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
            _sumbitting = true;
            _sumbitted = false;

            var isValid = true;
            if (string.IsNullOrWhiteSpace(LetterText)) isValid = false;

            if (isValid)
            {
                var newRequest = await TeamService.CreateRequestTeamToIdeaAsync(IdeaMarket, team, LetterText);
                _cachedRequests.Add(team.Id, newRequest);
                if (_queuedCollapseMethod is not null)
                {
                    await _queuedCollapseMethod();
                    _queuedCollapseMethod = null;
                }

                StateHasChanged();
            }

            _sumbitted = true;
            _sumbitting = false;
        }

        private void EventedRequestUpdate(Guid requestId, TeamRequestStatus newStatus)
        {
            foreach (var request in _cachedRequests)
                if (request.Value.Id == requestId)
                    request.Value.Status = newStatus;
            
            StateHasChanged();
        }

        public void Dispose()
        {
            TeamService.OnRequestsStatusUpdated -= EventedRequestUpdate;
        }
    }
}
