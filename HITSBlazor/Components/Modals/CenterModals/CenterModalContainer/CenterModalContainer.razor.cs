using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.CenterModals.CenterModalContainer
{
    public partial class CenterModalContainer : IDisposable
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        protected override void OnInitialized()
        {
            ModalService.OnCenterModalsUpdated += StateHasChanged;
        }

        private async Task CloseModalIfAllowed(bool isBlock)
        {
            if (!isBlock)
                await ModalService.Close(ModalType.Center);
        }

        public void Dispose()
        {
            ModalService.OnCenterModalsUpdated -= StateHasChanged;
        }
    }
}
