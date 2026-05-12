using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.CenterModals.MarketModal;
using HITSBlazor.Components.Tables.TableComponent;
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
    public partial class MarketsList
    {
        [Inject]
        private IMarketService MarketService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        private bool _isLoading = true;
        private TableComponent? _tableComponent;

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

            MarketService.OnMarketsHasCreated += MarketHasCreated;
            MarketService.OnMarketHasUpdated += MarketHasUpdated;
            MarketService.OnMarketStatusHasUpdated += MarketStatusHasUpdated;
            MarketService.OnMarketHasDeleted += MarketHasDeleted;

            await LoadMarketsAsync();

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async Task AdditionalAfterRenderMethod()
        {
            if (_tableComponent != null)
                _tableContainer = _tableComponent.ScrollContainer;
        }

        protected override async Task OnLoadMoreItemsAsync() => await LoadMarketsAsync(append: true);

        protected override int GetCurrentItemsCount() => _markets.Count;

        private async Task LoadMarketsAsync(bool append = false)
        {
            await LoadDataAsync(
                _markets,
                () => MarketService.GetMarketsAsync(
                    _currentPage,
                    searchText: _searchText,
                    selectedStatuses: [.. SelectedStatuses.Select(s => s.Value)],
                    orderBy: _orderBy,
                    byDescending: _byDescending
                ),
                append
            );
        }

        private static Dictionary<MenuAction, object> GetTableActions(Market market)
        {
            var actions = new Dictionary<MenuAction, object>
            {
                [MenuAction.GoToMarket] = market.Id
            };

            if (market.Status is MarketStatus.New)
                actions.Add(MenuAction.StartMarket, market);

            actions.Add(MenuAction.Edit, market);

            if (market.Status is MarketStatus.Active)
                actions.Add(MenuAction.FinishMarket, market);

            if (market.Status is MarketStatus.New or MarketStatus.Done)
                actions.Add(MenuAction.Delete, market);

            return actions;
        }

        private async Task FiltersHasChanged()
        {
            ResetPagination();
            await LoadMarketsAsync();
        }

        private async Task SearchMarket(string value)
        {
            _searchText = value;
            await FiltersHasChanged();
        }

        private async Task SortMarket(string? value, bool? state)
        {
            _orderBy = value;
            _byDescending = state;
            await FiltersHasChanged();
        }

        private async Task ResetFilters()
        {
            SelectedStatuses.Clear();

            foreach (var header in _marketTableHeader)
                header.IsOrdered = null;

            await FiltersHasChanged();
        }

        private async Task OpenMarket(Guid marketId)
            => await NavigationService.NavigateToAsync($"/market/{marketId}");

        private void ShowMarketModal(Market? market = null) => ModalService.Show<MarketModal>(
            ModalType.Center,
            parameters: market is not null
            ? new Dictionary<string, object> { [nameof(MarketModal.Market)] = market }
            : null
        );

        private async Task OnMarketAction(TableActionContext context)
        {
            if (context.Item is Guid marketId)
            {
                if (context.Action is MenuAction.GoToMarket)
                {
                    await OpenMarket(marketId);
                }
            }
            else if (context.Item is Market market)
            {
                if (context.Action is MenuAction.Edit)
                {
                    ShowMarketModal(market);
                }
                else if (context.Action is MenuAction.Delete)
                {
                    ModalService.ShowConfirmModal(
                        $"Вы действительно хотите удалить \"{market.Name}\"?",
                        () => MarketService.DeleteMarketAsync(market),
                        confirmButtonVariant: ButtonVariant.Danger,
                        confirmButtonText: "Удалить"
                    );
                }
                else if (context.Action is MenuAction.StartMarket)
                {
                    ModalService.ShowConfirmModal(
                        "Вы действительно хотите запустить биржу? Активную биржу можно будет ТОЛЬКО завершить.",
                        () => MarketService.UpdateMarketStatusAsync(market, MarketStatus.Active),
                        questionTextColor: TextColor.Danger,
                        confirmButtonVariant: ButtonVariant.Success,
                        confirmButtonText: "Запустить биржу"
                    );
                }
                else if (context.Action is MenuAction.FinishMarket)
                {
                    ModalService.ShowConfirmModal(
                        "Вы действительно хотите завершить биржу? Идеи, не нашедшие команды, попадут обратно в список идей.",
                        () => MarketService.UpdateMarketStatusAsync(market, MarketStatus.Done),
                        questionTextColor: TextColor.Danger,
                        confirmButtonVariant: ButtonVariant.Success,
                        confirmButtonText: "Завершить биржу"
                    );
                }
            }
        }

        private void MarketHasCreated(Market newMarket)
        {
            _markets.Add(newMarket);
            ++_totalCount;
            StateHasChanged();
        }

        private void MarketHasUpdated(Market updatedMarket)
        {
            var marketForUpdate = _markets.FirstOrDefault(m => m.Id == updatedMarket.Id);
            if (marketForUpdate is null) return;

            marketForUpdate.Name = updatedMarket.Name;
            marketForUpdate.StartDate = updatedMarket.StartDate;
            marketForUpdate.FinishDate = updatedMarket.FinishDate;
            StateHasChanged();
        }

        private void MarketStatusHasUpdated(Market market)
        {
            var marketForUpdate = _markets.FirstOrDefault(m => m.Id == market.Id);
            if (marketForUpdate is null) return;

            marketForUpdate.Status = market.Status;
            StateHasChanged();
        }

        private void MarketHasDeleted(Market market)
        {
            if (_markets.Remove(market))
            {
                --_totalCount;
                StateHasChanged();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            MarketService.OnMarketsHasCreated -= MarketHasCreated;
            MarketService.OnMarketHasUpdated -= MarketHasUpdated;
            MarketService.OnMarketStatusHasUpdated -= MarketStatusHasUpdated;
            MarketService.OnMarketHasDeleted -= MarketHasDeleted;

            await ValueTask.CompletedTask;
        }
    }
}
