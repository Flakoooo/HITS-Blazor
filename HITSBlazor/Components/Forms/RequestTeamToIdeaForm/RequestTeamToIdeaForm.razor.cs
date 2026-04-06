using HITSBlazor.Components.Button;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.IdeaMarkets;
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

        [Inject]
        private IIdeaMarketService IdeaMarketService { get; set; } = null!;

        [Parameter]
        public Guid IdeaMarketId { get; set; }

        private bool _isLoading = true;

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
                var requestsTeamToIdea = await IdeaMarketService.GetRequestsTeamToIdeaAsync(IdeaMarketId);

                _cachedRequests = (await IdeaMarketService.GetRequestsTeamToIdeaAsync(IdeaMarketId))
                    .Where(r => _teams.Select(t => t.Id).Contains(r.TeamId))
                    .ToDictionary(r => r.TeamId, r => r);

                _isLoading = false;
            }
        }

        private static ButtonVariant GetStatusButtonVariant(TeamRequestStatus status) => status switch
        {
            TeamRequestStatus.Canceled => ButtonVariant.Danger,
            TeamRequestStatus.Annulled => ButtonVariant.Secondary,
            _ => ButtonVariant.Primary
        };
    }
}
