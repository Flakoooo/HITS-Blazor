using HITSBlazor.Components.Modals.Components.RightSideModaCollapselInfo;
using HITSBlazor.Components.Modals.Components.RightSideModalInfo;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.IdeaMarkets;
using HITSBlazor.Services.Markets;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using HITSBlazor.Utils.EnumUIConverters;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.RightSideModals.IdeaMarketModal
{
    public partial class IdeaMarketModal : IDisposable
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private IIdeaMarketService IdeaMarketService { get; set; } = null!;

        [Inject]
        private ITeamService TeamService { get; set; } = null!;

        [Inject]
        private IMarketService MarketService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Parameter]
        public Guid IdeaMarketId { get; set; }

        private bool _isLoading = true;

        private Market? _currentMarket;
        private IdeaMarket? _currentIdeaMarket;

        private IdeaMarketTableCategory _activeTableCategory = IdeaMarketTableCategory.AcceptedTeam;

        private List<CollapseItem> _ideaData = [];

        private List<Skill> RequestTeamsSkills { get; set; } = [];

        private readonly List<RightSideModalInfoItem> _infoItems =
        [
            new RightSideModalInfoItem
            {
                Label = "Заказчик",
                Icon = "bi-person-circle",
                TextColor = Typography.TextColor.Primary
            },
            new RightSideModalInfoItem
            {
                Label = "Инициатор",
                Icon = "bi-envelope",
                TextColor = Typography.TextColor.Primary,
                IsLinkable = true
            },
            new RightSideModalInfoItem
            {
                Label = "Статус",
                Icon = "bi-check2-all",
                TextColor = Typography.TextColor.Primary
            },
            new RightSideModalInfoItem
            {
                Label = "Дата старта проекта",
                Icon = "bi-calendar",
                TextColor = Typography.TextColor.Primary
            },
            new RightSideModalInfoItem
            {
                Label = "Дата окончания проекта",
                Icon = "bi-calendar",
                TextColor = Typography.TextColor.Primary
            },
        ];

        protected override async Task OnInitializedAsync()
        {
            await LoadIdeaMarket();
        }

        private async Task LoadIdeaMarket()
        {
            _isLoading = true;
            StateHasChanged();

            _currentIdeaMarket = await IdeaMarketService.GetIdeaMarketAsync(IdeaMarketId);
            if (_currentIdeaMarket is null) return;

            TeamService.OnRequestTeamInIdeaStatusUpdated += RequestHasAccepted;

            _ideaData = GetIdeaData();

            _infoItems[0].Text = _currentIdeaMarket.Customer;

            _infoItems[1].Text = _currentIdeaMarket.Initiator.FullName;

            var initiatorInfoItem = _infoItems[1];
            initiatorInfoItem.Text = _currentIdeaMarket.Initiator.FullName;
            initiatorInfoItem.LinkMethod = () => ModalService.ShowProfileModal(_currentIdeaMarket.Initiator.Id);

            _infoItems[2].Text = EnumUIConverter.GetInfo(_currentIdeaMarket.Status).DisplayText;

            _currentMarket = await MarketService.GetMarketByIdAsync(_currentIdeaMarket.MarketId);
            _infoItems[3].Text = _currentMarket?.StartDate.ToString("dd.MM.yyyy") ?? "-";
            _infoItems[4].Text = _currentMarket?.FinishDate.ToString("dd.MM.yyyy") ?? "-";

            _isLoading = false;
        }

        private List<CollapseItem> GetIdeaData() => [
            new() { Title = "Проблема",                                     Data = _currentIdeaMarket?.Problem        },
            new() { Title = "Предлагаемое решение",                         Data = _currentIdeaMarket?.Solution       },
            new() { Title = "Ожидаемый результат",                          Data = _currentIdeaMarket?.Result         },
            new() { Title = "Описание необходимых ресурсов для реализации", Data = _currentIdeaMarket?.Description    }
        ];

        private static string GetCategoryInfo(IdeaMarketTableCategory category) => category switch
        {
            IdeaMarketTableCategory.AcceptedTeam => "Принятая команда",
            IdeaMarketTableCategory.Requests => "Заявки",
            IdeaMarketTableCategory.InvitedTeams => "Приглашенные команды",
            _ => category.ToString()
        };

        private string GetTableCategoryClass(IdeaMarketTableCategory category)
            => _activeTableCategory == category ? "active text-primary" : "text-secondary";

        private async Task ChangeCategory(IdeaMarketTableCategory category)
        {
            _activeTableCategory = category;
        }

        private async Task InviteTeams()
        {
            await ModalService.CloseAll(ModalType.RightSide);
            await NavigationService.NavigateToAsync("/teams/list");
        }

        private async Task RequestHasAccepted(Guid requestId, TeamRequestStatus newStatus)
        {
            if (_currentIdeaMarket is not null && newStatus is TeamRequestStatus.Accepted)
            {
                await LoadIdeaMarket();
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            TeamService.OnRequestTeamInIdeaStatusUpdated -= RequestHasAccepted;
        }
    }
}
