using HITSBlazor.Models.Tests.Entities;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.RightSideModals.TestModal
{
    public partial class TestModal
    {
        [Parameter]
        public Test? CurrentTest { get; set; }

        private bool _isLoading = true;

        private bool IsLoading => _isLoading || CurrentTest is null;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;



            _isLoading = false;
        }
    }
}
