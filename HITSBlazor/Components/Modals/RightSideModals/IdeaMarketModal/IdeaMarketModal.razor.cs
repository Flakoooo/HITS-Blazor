using HITSBlazor.Components.Modals.Components;
using HITSBlazor.Models.Markets.Entities;

namespace HITSBlazor.Components.Modals.RightSideModals.IdeaMarketModal
{
    public partial class IdeaMarketModal
    {
        private bool _isLoading = true;
        private RightSideModalCategory _activeInfoCategory = RightSideModalCategory.Info;

        private IdeaMarket? _currentIdeaMarket;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;


            _isLoading = false;
        }

        private string GetInfoCategoryClass(RightSideModalCategory category)
            => _activeInfoCategory == category ? "btn-primary" : "btn-secondary";
    }
}
