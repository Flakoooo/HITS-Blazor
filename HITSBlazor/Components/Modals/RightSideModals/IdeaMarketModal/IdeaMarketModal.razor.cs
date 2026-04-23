using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Modals.CenterModals.LetterModal;
using HITSBlazor.Components.Modals.Components.RightSideModaCollapselInfo;
using HITSBlazor.Components.Modals.Components.RightSideModalInfo;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.IdeaMarkets;
using HITSBlazor.Services.Markets;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using HITSBlazor.Utils.EnumUIConverters;
using HITSBlazor.Utils.Models;
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

        private string _searchText = string.Empty;

        private List<RequestTeamToIdea> _requestsTeamsToIdea = [];
        private List<InvitationTeamToIdea> _invitationsTeamsToIdea = [];

        private IdeaMarketTableCategory _activeTableCategory = IdeaMarketTableCategory.AcceptedTeam;

        private List<CollapseItem> _ideaData = [];

        private List<Skill> RequestTeamsSkills { get; set; } = [];

        private static List<TableHeaderItem> AcceptedTeamTableHeader { get; } =
        [
            new() { Text = "" },
            new() { Text = "Название", ColumnClass = "col-3" },
            new() { Text = "Лидер", ColumnClass = "col-3" },
            new() { Text = "Участники", InCentered = true, OrderBy = nameof(Team.MembersCount) },
            new() { Text = "Компетенции", InCentered = true, ColumnClass = "col-4" }
        ];

        private static List<TableHeaderItem> RequestsTableHeader { get; } =
        [
            new() { Text = "" },
            new() { Text = "Название", ColumnClass = "col-3" },
            new() { Text = "Статус", ColumnClass = "col-3" },
            new() { Text = "Участники", InCentered = true, OrderBy = nameof(Team.MembersCount) },
            new() { Text = "Компетенции", InCentered = true, ColumnClass = "col-4" }
        ];

        private static List<TableHeaderItem> InvitedTeamsTableHeader { get; } =
        [
            new() { Text = "" },
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

            await LoadDataAsync();
            TeamService.OnRequestsStatusCreated += LoadDataAsync;

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

        private async Task LoadDataAsync()
        {
            if (_currentIdeaMarket is null) return;

            if (_activeTableCategory is IdeaMarketTableCategory.Requests)
                _requestsTeamsToIdea = await IdeaMarketService.GetRequestsTeamToIdeaAsync(
                    _currentIdeaMarket.Id,
                    searchText: _searchText
                );
            else if (_activeTableCategory is IdeaMarketTableCategory.InvitedTeams)
                _invitationsTeamsToIdea = await IdeaMarketService.GetInvitationTeamsToIdeaAsync(
                    _currentIdeaMarket.IdeaId,
                    searchText: _searchText
                );
        }

        private async Task ChangeCategory(IdeaMarketTableCategory category)
        {
            _activeTableCategory = category;
            await LoadDataAsync();
        }

        private void ShowTeamModal(Guid teamId) => ModalService.ShowTeamModal(teamId);

        private async Task SearchData(string value)
        {
            _searchText = value;
            await LoadDataAsync();
        }

        private void HandleTableMenuClick(TableActionContext context)
        {
            if (context.Action == MenuAction.ViewTeamProfile)
            {
                ShowTeamModal((Guid)context.Item);
            }
            if (context.Action == MenuAction.ViewLetter)
            {
                if (_currentIdeaMarket is not null && AuthService.CurrentUser is not null)
                {
                    //TODO: сделать назначение команды
                    bool buttonAllowed = _currentIdeaMarket.Status is not IdeaMarketStatusType.RecruitmentIsClosed
                        && (AuthService.CurrentUser.Id == _currentIdeaMarket.Initiator.Id || AuthService.CurrentUser.Role is RoleType.Admin);
                    ModalService.Show<LetterModal>(
                        ModalType.Center,
                        parameters: new Dictionary<string, object>
                        {
                            [nameof(LetterModal.Letter)] = context.Item,
                            [nameof(LetterModal.AcceptedButtonAllowed)] = buttonAllowed,
                            //[nameof(LetterModal.OnAcceptedButtonClick)] = 
                        });
                }
            }
        }

        public void Dispose()
        {
            TeamService.OnRequestsStatusCreated -= LoadDataAsync;
        }
    }
}
