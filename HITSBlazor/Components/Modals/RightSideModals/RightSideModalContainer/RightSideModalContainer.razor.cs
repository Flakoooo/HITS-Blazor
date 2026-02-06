using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.RightSideModals.RightSideModalContainer
{
    public partial class RightSideModalContainer : IDisposable
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private bool _isContainerVisible;
        private bool _isContainerClosing;
        private bool _isModalClosing;
        private bool _isBlockCloseModal;
        private Type? _currentComponentType;
        private Dictionary<string, object> _parameters = [];

        protected override void OnInitialized()
        {
            ModalService.OnShowSideModal += ShowModal;
            ModalService.OnCloseSideModal += StartModalCloseAnimation;
            ModalService.OnCloseSideModalContainer += StartContainerCloseAnimation;
        }

        private async void ShowModal(ModalData modalData)
        {
            if (_isContainerClosing)
                _isContainerClosing = false;

            if (_isModalClosing)
            {
                await Task.Delay(150);
                _isModalClosing = false;
            }

            _currentComponentType = modalData.ComponentType;
            _isBlockCloseModal = modalData.BlockCloseModal;
            _parameters = modalData.Parameters ?? [];

            if (!_isContainerVisible)
            {
                _isContainerVisible = true;
                await Task.Delay(10);
            }

            StateHasChanged();
        }

        private void CloseModalIfAllowed()
        {
            if (!_isBlockCloseModal)
                ModalService.Close(ModalType.RightSide);
        }

        private async void StartModalCloseAnimation()
        {
            _isModalClosing = true;
            StateHasChanged();

            await Task.Delay(150);

            _currentComponentType = null;
            _parameters.Clear();
            _isModalClosing = false;

            StateHasChanged();
        }

        private async void StartContainerCloseAnimation()
        {
            _isContainerClosing = true;
            StateHasChanged();

            await Task.Delay(150);

            _isContainerVisible = false;
            _currentComponentType = null;
            _parameters.Clear();
            _isContainerClosing = false;
            _isModalClosing = false;

            StateHasChanged();
        }

        public void Dispose()
        {
            ModalService.OnShowSideModal -= ShowModal;
            ModalService.OnCloseSideModal -= StartModalCloseAnimation;
            ModalService.OnCloseSideModalContainer -= StartContainerCloseAnimation;
        }
    }
}
