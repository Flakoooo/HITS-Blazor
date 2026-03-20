using HITSBlazor.Components.Button;
using HITSBlazor.Components.Typography;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Services.IdeaMarkets;
using HITSBlazor.Services.Markets;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;

namespace HITSBlazor.Pages.Markets.MarketIdeas
{
    [Authorize]
    [Route("market/{MarketId}")]
    public partial class MarketIdeas
    {
        [Inject]
        private IMarketService MarketService { get; set; } = null!;

        [Inject]
        private IIdeaMarketService IdeaMarketService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public string MarketId { get; set; } = string.Empty;

        private bool _isLoading = true;
        private string _searchText = string.Empty;

        private MarketIdeasCategory _category = MarketIdeasCategory.All;

        private Market? _currentMarket;

        private List<IdeaMarket> _ideaMarkets = [];


        private IdeaMarketStatusType? _selectedStatusType;
        private string SeacrhSkillText { get; set; } = string.Empty;
        private HashSet<Guid> SelectedSkillIds { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (!string.IsNullOrWhiteSpace(MarketId) && Guid.TryParse(MarketId, out Guid guid))
            {
                _currentMarket = await MarketService.GetMarketByIdAsync(guid);

                _ideaMarkets = await IdeaMarketService.GetIdeasMarketAsync(guid);

                _isLoading = false;
            }
        }

        private async Task LoadMarketIdeasAsync()
        {
            if (_currentMarket is null) return;

            _ideaMarkets = await IdeaMarketService.GetIdeasMarketAsync(
                _currentMarket.Id,
                searchText: _searchText,
                selectedStatus: _selectedStatusType
            );
        }

        private string GetActiveCategoryClass(MarketIdeasCategory category)
            => category == _category? "active text-primary" : "text-dark";

        private void SelectActiveCategory(MarketIdeasCategory category)
            => _category = category;

        private async Task SearchMarketIdeas(string value)
        {
            _searchText = value;
            await LoadMarketIdeasAsync();
        }

        private async Task CloseMarket()
        {
            if (_currentMarket is null)
                return;

            ModalService.ShowConfirmModal(
                "Вы действительно хотите завершить биржу? Идеи, не нашедшие команды, попадут обратно в список идей.",
                () => MarketService.UpdateMarketStatusAsync(_currentMarket.Id, MarketStatus.Done),
                questionTextColor: TextColor.Danger,
                confirmButtonVariant: ButtonVariant.Success,
                confirmButtonText: "Завершить биржу"
            );
        }

        private async Task ResetFilters()
        {
            SeacrhSkillText = string.Empty;
            SelectedSkillIds.Clear();
            await LoadMarketIdeasAsync();
        }
    }
}
