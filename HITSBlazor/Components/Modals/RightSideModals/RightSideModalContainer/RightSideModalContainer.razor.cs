using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.RightSideModals.RightSideModalContainer
{
    public partial class RightSideModalContainer : IDisposable
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        protected override void OnInitialized()
        {
            ModalService.OnRightSideModalsUpdated += StateHasChanged;
        }

        private async Task CloseModalIfAllowed(bool isBlock)
        {
            if (!isBlock)
                await ModalService.Close(ModalType.RightSide);
        }

        public void Dispose()
        {
            ModalService.OnRightSideModalsUpdated -= StateHasChanged;
        }
    }
}
