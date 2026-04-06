using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Modals.Components.RightSideModaCollapselInfo;
using HITSBlazor.Components.Modals.Components.RightSideModalInfo;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.IdeaMarkets;
using HITSBlazor.Services.Markets;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using HITSBlazor.Utils.Models;
using Microsoft.AspNetCore.Components;
using System;

namespace HITSBlazor.Components.Modals.RightSideModals.IdeaMarketModal
{
    public partial class IdeaMarketModal
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Inject]
        private IIdeaMarketService IdeaMarketService { get; set; } = null!;

        [Inject]
        private IMarketService MarketService { get; set; } = null!;

        [Inject]
        private ITeamService TeamService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public Guid IdeaMarketId { get; set; }

        private bool _isLoading = true;

        private Market? _currentMarket;
        private IdeaMarket? _currentIdeaMarket;

        private List<RequestTeamToIdea> _requestsTeamToIdeas = [];
        private List<InvitationTeamToIdea> _invitationsTeamToIdeas = [];

        private IdeaMarketTableCategory _activeTableCategory = IdeaMarketTableCategory.AcceptedTeam;

        private List<CollapseItem> _ideaData = [];

        private static List<TableHeaderItem> AcceptedTeamTableHeader { get; } =
        [
            //сделать поле с чекбоксом
            new() { Text = "Название", ColumnClass = "col-3" },
            new() { Text = "Лидер", ColumnClass = "col-3" },
            new() { Text = "Участники", InCentered = true, OrderBy = nameof(Team.MembersCount) },
            new() { Text = "Компетенции", InCentered = true, ColumnClass = "col-4" }
        ];

        private static List<TableHeaderItem> RequestsTableHeader { get; } =
        [
            //сделать поле с чекбоксом
            new() { Text = "Название", ColumnClass = "col-3" },
            new() { Text = "Статус", ColumnClass = "col-3" },
            new() { Text = "Участники", InCentered = true, OrderBy = nameof(Team.MembersCount) },
            new() { Text = "Компетенции", InCentered = true, ColumnClass = "col-4" }
        ];

        private static List<TableHeaderItem> InvitedTeamsTableHeader { get; } =
        [
            //сделать поле с чекбоксом
            new() { Text = "Название", ColumnClass = "col-3" },
            new() { Text = "Статус", ColumnClass = "col-3" },
            new() { Text = "Участники", InCentered = true, OrderBy = nameof(Team.MembersCount) },
            new() { Text = "Компетенции", InCentered = true, ColumnClass = "col-4" }
        ];

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
            _isLoading = true;
            _currentIdeaMarket = await IdeaMarketService.GetIdeaMarketAsync(IdeaMarketId);
            if (_currentIdeaMarket is null) return;

            _ideaData = GetIdeaData();

            if (_currentIdeaMarket.Team is not null)
                _requestsTeamToIdeas = await TeamService.GetRequestsTeamToIdeasAsync(_currentIdeaMarket.Team.Id);

            _infoItems[0].Text = _currentIdeaMarket.Customer;

            _infoItems[1].Text = _currentIdeaMarket.Initiator.FullName;

            var initiatorInfoItem = _infoItems[1];
            initiatorInfoItem.Text = _currentIdeaMarket.Initiator.FullName;
            initiatorInfoItem.LinkMethod = () => ModalService.ShowProfileModal(_currentIdeaMarket.Initiator.Id);

            _infoItems[2].Text = EnumViewModel<IdeaMarketStatusType>.GetTranslation(_currentIdeaMarket.Status);

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

        private string GetTableCategoryClass(IdeaMarketTableCategory category)
            => _activeTableCategory == category ? "active text-primary" : "text-secondary";

        private void ShowTeamModal(Guid teamId) => ModalService.ShowTeamModal(teamId);

        private void HandleTableMenuClick(TableActionContext context)
        {
            if (context.Action == MenuAction.ViewTeamProfile)
            {
                ShowTeamModal((Guid)context.Item);
            }
            if (context.Action == MenuAction.ViewLetter)
            {
                
            }
        }
    }
}
