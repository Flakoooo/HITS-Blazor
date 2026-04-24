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
            ModalService.OnAllModalsUpdated += StateHasChanged;
            ModalService.OnRightSideModalsUpdated += StateHasChanged;
        }

        private async Task CloseModalIfAllowed(bool isBlock)
        {
            if (!isBlock)
                await ModalService.Close(ModalType.RightSide);
        }

        private int GetZIndexValue()
        {
            var type = ModalService.AllModals.Peek().Type;
            var index = ModalService.AllModals.Count * (type is ModalType.RightSide ? 600 : 500);
            if (type is ModalType.Center) index -= 500;

            return index;
        }

        public void Dispose()
        {
            ModalService.OnAllModalsUpdated -= StateHasChanged;
            ModalService.OnRightSideModalsUpdated -= StateHasChanged;
        }
    }
}
