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
            ModalService.OnAllModalsUpdated += StateHasChanged;
            ModalService.OnCenterModalsUpdated += StateHasChanged;
        }

        private async Task CloseModalIfAllowed(bool isBlock)
        {
            if (!isBlock)
                await ModalService.Close(ModalType.Center);
        }

        private int GetZIndexValue(int count)
        {
            if (count == ModalService.CenterModals.Count)
                return count * (ModalService.AllModals.Peek().Type is ModalType.Center ? 600 : 500);

            return count * 500;
        }

        public void Dispose()
        {
            ModalService.OnAllModalsUpdated -= StateHasChanged;
            ModalService.OnCenterModalsUpdated -= StateHasChanged;
        }
    }
}
