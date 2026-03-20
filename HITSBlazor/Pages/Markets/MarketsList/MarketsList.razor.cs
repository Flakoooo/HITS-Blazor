using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.CenterModals.MarketModal;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Components.Typography;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Markets;
using HITSBlazor.Services.Modal;
using HITSBlazor.Utils.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Markets.MarketsList
{
    [Authorize]
    [Route("market/list")]
    public partial class MarketsList : IDisposable
    {
        [Inject]
        private IMarketService MarketService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        private bool _isLoading = true;

        private static readonly List<TableHeaderItem> _marketTableHeader =
        [
            new() { Text = "Название",          ColumnClass = "col-4"                                   },
            new() { Text = "Статус",            InCentered = true                                       },
            new() { Text = "Дата старта",       InCentered = true, OrderBy = nameof(Market.StartDate)   },
            new() { Text = "Дата окончания",    InCentered = true, OrderBy = nameof(Market.FinishDate)  }
        ];

        private readonly List<EnumViewModel<MarketStatus>> _filterMarketStatus
            = [.. Enum.GetValues<MarketStatus>().Select(s => new EnumViewModel<MarketStatus>(s))];

        private string _searchText = string.Empty;
        private string? _orderBy = null;
        private bool? _byDescending = null;

        private List<Market> _markets = [];

        private HashSet<EnumViewModel<MarketStatus>> SelectedStatuses { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            await LoadMarketsAsync();
            MarketService.OnMarketsStateUpdated += StateHasChanged;
            MarketService.OnMarketsStateChanged += LoadMarketsAsync;

            _isLoading = false;
        }

        private async Task LoadMarketsAsync()
        {
            _markets = await MarketService.GetMarketsAsync(
                searchText: _searchText, 
                selectedStatuses: [.. SelectedStatuses.Select(s => s.Value)],
                orderBy: _orderBy,
                byDescending: _byDescending
            );

            StateHasChanged();
        }

        private static Dictionary<MenuAction, object> GetTableActions(Market market)
        {
            var actions = new Dictionary<MenuAction, object>
            {
                [MenuAction.GoToMarket] = market.Id
            };

            if (market.Status is MarketStatus.New)
                actions.Add(MenuAction.StartMarket, market.Id);

            actions.Add(MenuAction.Edit, market);

            if (market.Status is MarketStatus.Active)
                actions.Add(MenuAction.FinishMarket, market.Id);

            if (market.Status is MarketStatus.New or MarketStatus.Done)
                actions.Add(MenuAction.Delete, market);

            return actions;
        }

        private async Task SearchMarket(string value)
        {
            _searchText = value;
            await LoadMarketsAsync();
        }

        private async Task SortMarket(string? value, bool? state)
        {
            _orderBy = value;
            _byDescending = state;
            await LoadMarketsAsync();
        }

        private async Task ResetFilters()
        {
            SelectedStatuses.Clear();

            await LoadMarketsAsync();
        }

        private async Task OpenMarket(Guid marketId)
            => await NavigationService.NavigateToAsync($"/market/{marketId}");

        private void ShowMarketModal(Market? market = null)
        {
            if (market is not null)
            {
                ModalService.Show<MarketModal>(
                    ModalType.Center,
                    parameters: new Dictionary<string, object> { [nameof(MarketModal.Market)] = market }
                );
            }
            else
            {
                ModalService.Show<MarketModal>(ModalType.Center);
            }
        }

        private async Task OnMarketAction(TableActionContext context)
        {
            if (context.Action == MenuAction.GoToMarket)
            {
                if (context.Item is Guid marketId)
                    await OpenMarket(marketId);

            }
            else if (context.Action == MenuAction.StartMarket)
            {
                if (context.Item is Guid marketId)
                    ModalService.ShowConfirmModal(
                        "Вы действительно хотите запустить биржу? Активную биржу можно будет ТОЛЬКО завершить.",
                        () => MarketService.UpdateMarketStatusAsync(marketId, MarketStatus.Active),
                        questionTextColor: TextColor.Danger,
                        confirmButtonVariant: ButtonVariant.Success,
                        confirmButtonText: "Запустить биржу"
                    );
            }
            else if (context.Action == MenuAction.Edit)
            {
                if (context.Item is Market market)
                    ShowMarketModal(market);
            }
            else if (context.Action == MenuAction.FinishMarket)
            {
                if (context.Item is Guid marketId)
                    ModalService.ShowConfirmModal(
                        "Вы действительно хотите завершить биржу? Идеи, не нашедшие команды, попадут обратно в список идей.",
                        () => MarketService.UpdateMarketStatusAsync(marketId, MarketStatus.Done),
                        questionTextColor: TextColor.Danger,
                        confirmButtonVariant: ButtonVariant.Success,
                        confirmButtonText: "Завершить биржу"
                    );
            }
            else if (context.Action == MenuAction.Delete)
            {
                if (context.Item is Market market)
                    ModalService.ShowConfirmModal(
                        $"Вы действительно хотите удалить \"{market.Name}\"?",
                        () => MarketService.DeleteMarketAsync(market),
                        confirmButtonVariant: ButtonVariant.Danger,
                        confirmButtonText: "Удалить"
                    );
            }
        }

        public void Dispose()
        {
            MarketService.OnMarketsStateUpdated -= StateHasChanged;
            MarketService.OnMarketsStateChanged -= LoadMarketsAsync;
        }
    }
}
