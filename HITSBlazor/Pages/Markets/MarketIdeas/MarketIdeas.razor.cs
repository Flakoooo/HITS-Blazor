using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.RightSideModals.IdeaMarketModal;
using HITSBlazor.Components.Modals.RightSideModals.RequestToIdeaModal;
using HITSBlazor.Components.Typography;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.IdeaMarkets;
using HITSBlazor.Services.Markets;
using HITSBlazor.Services.Modal;
using HITSBlazor.Utils.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Markets.MarketIdeas
{
    [Authorize]
    [Route("market/{MarketId}")]
    public partial class MarketIdeas
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

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

        private readonly List<EnumViewModel<IdeaMarketStatusType>> _filterIdeaMarketStatus
            = [.. Enum.GetValues<IdeaMarketStatusType>().Select(s => new EnumViewModel<IdeaMarketStatusType>(s))];
        private EnumViewModel<IdeaMarketStatusType>? SelectedStatusType { get; set; }
        private string SeacrhSkillText { get; set; } = string.Empty;
        private HashSet<Guid> SelectedSkillIds { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (!string.IsNullOrWhiteSpace(MarketId) && Guid.TryParse(MarketId, out Guid guid))
            {
                _currentMarket = await MarketService.GetMarketByIdAsync(guid);

                await LoadMarketIdeasAsync();

                _isLoading = false;
            }

            IdeaMarketService.OnIdeasMarketStateUpdated += StateHasChanged;
            IdeaMarketService.OnIdeasMarketStateChanged += LoadMarketIdeasAsync;
        }

        private async Task LoadMarketIdeasAsync()
        {
            if (_currentMarket is null) return;

            _ideaMarkets = await IdeaMarketService.GetIdeasMarketAsync(
                _currentMarket.Id,
                favorite: _category == MarketIdeasCategory.Favorite ? true : null,
                searchText: _searchText,
                selectedStatus: SelectedStatusType?.Value
            );
        }

        private string GetActiveCategoryClass(MarketIdeasCategory category)
            => category == _category? "active text-primary" : "text-dark";

        private async Task SelectActiveCategory(MarketIdeasCategory category)
        {
            _category = category;
            await LoadMarketIdeasAsync();
        }

        private async Task SearchMarketIdeas(string value)
        {
            _searchText = value;
            await LoadMarketIdeasAsync();
        }

        private async Task CloseMarket()
        {
            if (_currentMarket is null) return;

            ModalService.ShowConfirmModal(
                "Вы действительно хотите завершить биржу? Идеи, не нашедшие команды, попадут обратно в список идей.",
                () => MarketService.UpdateMarketStatusAsync(_currentMarket.Id, MarketStatus.Done),
                questionTextColor: TextColor.Danger,
                confirmButtonVariant: ButtonVariant.Success,
                confirmButtonText: "Завершить биржу"
            );
        }

        private async Task ChangeIdeaFavorite(IdeaMarket ideaMarket)
        {
            if (AuthService.CurrentUser is not null)
            {
                if (ideaMarket.IsFavorite)
                {
                    var result = await IdeaMarketService.UnsetIdeaFromFavorite(AuthService.CurrentUser.Id, ideaMarket);
                    if (result)
                    {
                        ideaMarket.IsFavorite = false; 
                        StateHasChanged();
                    }
                }
                else
                {
                    var result = await IdeaMarketService.SetIdeaFavorite(AuthService.CurrentUser.Id, ideaMarket);
                    if (result)
                    {
                        ideaMarket.IsFavorite = true;
                        StateHasChanged();
                    }
                }
            }
        }

        private async Task SetIdeaFavorite(IdeaMarket ideaMarket)
        {
            if (AuthService.CurrentUser is not null)
            {
                var result = await IdeaMarketService.SetIdeaFavorite(AuthService.CurrentUser.Id, ideaMarket);
                if (result) ideaMarket.IsFavorite = result;
                StateHasChanged();
            }
        }

        private async Task UnsetIdeaFromFavorite(IdeaMarket ideaMarket)
        {
            if (AuthService.CurrentUser is not null)
            {
                var result = await IdeaMarketService.UnsetIdeaFromFavorite(AuthService.CurrentUser.Id, ideaMarket);
                if (result) ideaMarket.IsFavorite = !result;
                StateHasChanged();
            }
        }

        private void ShowIdeaMarketModal(Guid ideaMarketId) => ModalService.Show<IdeaMarketModal>(
            ModalType.RightSide,
            parameters: new Dictionary<string, object>
            {
                [nameof(IdeaMarketModal.IdeaMarketId)] = ideaMarketId
            }
        );

        private void ShowCreateTeamRequestModal(IdeaMarket ideaMarket) => ModalService.Show<RequestToIdeaModal>(
            ModalType.RightSide,
            parameters: new Dictionary<string, object>
            {
                [nameof(RequestToIdeaModal.CurrentIdeaMarket)] = ideaMarket
            }
        );

        private async Task ResetFilters()
        {
            SeacrhSkillText = string.Empty;
            SelectedSkillIds.Clear();
            await LoadMarketIdeasAsync();
        }
    }
}
