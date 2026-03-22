using HITSBlazor.Models.Tests.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.RightSideModals.TestModal
{
    public partial class TestModal
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Parameter]
        public Test? CurrentTest { get; set; }

        private bool _isLoading = true;

        private bool IsLoading => _isLoading || CurrentTest is null;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;



            _isLoading = false;
        }

        private async Task GoToTestResults()
        {
            if (CurrentTest is null) return;

            await ModalService.Close(ModalType.RightSide);

            await NavigationService.NavigateToAsync($"tests/{CurrentTest.TestName}/result/all");
        }
    }
}
