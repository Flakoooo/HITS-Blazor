using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.RightSideModals.IdeaMarketModal;
using HITSBlazor.Components.Modals.RightSideModals.RequestToIdeaModal;
using HITSBlazor.Components.Typography;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.IdeaMarkets;
using HITSBlazor.Services.Markets;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
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
        private IProjectService ProjectService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

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

            MarketService.OnMarketStatusHasUpdated += MarketStatusHasChanged;

            if (!string.IsNullOrWhiteSpace(MarketId) && Guid.TryParse(MarketId, out Guid guid))
            {
                _currentMarket = await MarketService.GetMarketByIdAsync(guid);

                await LoadMarketIdeasAsync();

                _isLoading = false;
                MarkAsInitialized();
            }
        }

        protected override async Task OnLoadMoreItemsAsync() 
            => await LoadMarketIdeasAsync(append: true);

        protected override int GetCurrentItemsCount() => _ideaMarkets.Count;

        private async Task LoadMarketIdeasAsync(bool append = false)
        {
            if (_currentMarket is null) return;

            await LoadDataAsync(
                _ideaMarkets,
                () => IdeaMarketService.GetIdeasMarketAsync(
                    _currentPage,
                    _currentMarket.Id,
                    favorite: _category == MarketIdeasCategory.Favorite ? true : null,
                    searchText: _searchText,
                    selectedStatus: SelectedStatusType?.Value
                ),
                append: append
            );
        }

        private async Task FiltersHasChanged()
        {
            ResetPagination();
            await LoadMarketIdeasAsync();
        }

        private async Task SelectActiveCategory(MarketIdeasCategory category)
        {
            _category = category;
            await FiltersHasChanged();
        }

        private async Task SearchMarketIdeas(string value)
        {
            _searchText = value;
            await FiltersHasChanged();
        }

        private async Task CloseMarket()
        {
            if (_currentMarket is null) return;

            ModalService.ShowConfirmModal(
                "Вы действительно хотите завершить биржу? Идеи, не нашедшие команды, попадут обратно в список идей.",
                () => MarketService.UpdateMarketStatusAsync(_currentMarket, MarketStatus.Done),
                questionTextColor: TextColor.Danger,
                confirmButtonVariant: ButtonVariant.Success,
                confirmButtonText: "Завершить биржу"
            );
        }

        private async Task ChangeIdeaFavorite(IdeaMarket ideaMarket)
        {
            if (AuthService.CurrentUser is null) return;

            if (ideaMarket.IsFavorite)
            {
                if (await IdeaMarketService.UnsetIdeaFromFavorite(AuthService.CurrentUser.Id, ideaMarket))
                {
                    ideaMarket.IsFavorite = false;
                    StateHasChanged();
                }
            }
            else
            {
                if (await IdeaMarketService.SetIdeaFavorite(AuthService.CurrentUser.Id, ideaMarket))
                {
                    ideaMarket.IsFavorite = true;
                    StateHasChanged();
                }
            }
        }

        private async Task ConvertIdeaToProject(IdeaMarket ideaMarket)
        {
            if (ideaMarket.Team is null || ideaMarket.Status is not IdeaMarketStatusType.RecruitmentIsClosed) return;

            if (await ProjectService.CreateNewProjectAsync(ideaMarket))
                ideaMarket.Status = IdeaMarketStatusType.Project;
        }

        private void ShowIdeaMarketModal(Guid ideaMarketId) => ModalService.ShowIdeaMarketModal(ideaMarketId);

        private void ShowCreateTeamRequestModal(IdeaMarket ideaMarket) => ModalService.Show<RequestToIdeaModal>(
            ModalType.RightSide,
            parameters: new Dictionary<string, object>
            {
                [nameof(RequestToIdeaModal.CurrentIdeaMarket)] = ideaMarket
            }
        );

        private async void MarketStatusHasChanged(Market market)
        {
            if (_currentMarket?.Id != market.Id) return;

            if (market.Status is MarketStatus.Done)
                await NavigationService.NavigateToAsync("market/list");

        }

        private async Task ResetFilters()
        {
            SeacrhSkillText = string.Empty;
            SelectedSkillIds.Clear();
            await FiltersHasChanged();
        }

        protected override ValueTask DisposeAsyncCore()
        {
            MarketService.OnMarketStatusHasUpdated -= MarketStatusHasChanged;

            return ValueTask.CompletedTask;
        }
    }
}
