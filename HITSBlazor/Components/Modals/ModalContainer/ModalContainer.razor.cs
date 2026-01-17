using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.ModalContainer
{
    public partial class ModalContainer : IDisposable
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private bool IsVisible { get; set; }
        private bool IsClosing { get; set; } = false;
        private bool IsBlockCloseModal { get; set; }
        private Type? CurrentComponentType { get; set; }
        private Dictionary<string, object> Parameters { get; set; } = new();

        protected override void OnInitialized()
        {
            ModalService.OnShow += ShowModal;
            ModalService.OnClose += CloseModal;
        }

        private async void ShowModal(ModalData modalData)
        {
            IsBlockCloseModal = modalData.BlockCloseModal;
            IsVisible = true;
            CurrentComponentType = modalData.ComponentType;
            Parameters = modalData.Parameters;

            IsClosing = false;
            await Task.Delay(10);
            StateHasChanged();
        }

        private void CloseModalIfAllowed()
        {
            if (!IsBlockCloseModal)
                CloseModal();
        }

        private async void CloseModal()
        {
            IsClosing = true;
            StateHasChanged();
            await Task.Delay(100);

            IsVisible = false;
            CurrentComponentType = null;
            Parameters.Clear();
            StateHasChanged();
        }

        public void Dispose()
        {
            ModalService.OnShow -= ShowModal;
            ModalService.OnClose -= CloseModal;
        }
    }
}
