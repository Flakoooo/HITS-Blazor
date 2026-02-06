using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Services.Modal
{
    public class ModalService
    {
        public event Action<ModalData>? OnShowCenterModal;
        public event Action<ModalData>? OnShowSideModal;

        public event Action? OnCloseCenterModal;
        public event Action? OnCloseSideModal;

        public event Action? OnCloseCenterModalContainer;
        public event Action? OnCloseSideModalContainer;

        private Stack<ModalData> _centerModals = [];
        private Stack<ModalData> _sideModals = [];

        private ModalData? _currentCenterModal;
        private ModalData? _currentSideModal;

        public bool HasActiveCenterModal => _currentCenterModal != null;
        public bool HasActiveSideModal => _currentSideModal != null;

        public void Show<TComponent>(
            ModalType type,
            bool blockCloseModal = false, 
            Dictionary<string, object>? parameters = null,
            string? customClass = null
        ) where TComponent : ComponentBase
        {
            Show(typeof(TComponent), type, blockCloseModal, parameters, customClass);
        }

        public void Show(
            Type componentType,
            ModalType type,
            bool blockCloseModal = false,
            Dictionary<string, object>? parameters = null,
            string? customClass = null
        )
        {
            var modalData = new ModalData
            {
                ComponentType = componentType,
                BlockCloseModal = blockCloseModal,
                Parameters = parameters ?? [],
                Type = type,
                CustomClass = customClass
            };

            switch (type)
            {
                case ModalType.Center:
                    _centerModals.Push(modalData);
                    _currentCenterModal = modalData;
                    OnShowCenterModal?.Invoke(modalData);
                    break;

                case ModalType.RightSide:
                    _sideModals.Push(modalData);
                    _currentSideModal = modalData;
                    OnShowSideModal?.Invoke(modalData);
                    break;

                default:
                    break;
            }
        }

        public async void Close(ModalType type)
        {
            switch (type)
            {
                case ModalType.Center:
                    if (_centerModals.Count == 0)
                        return;

                    _centerModals.Pop();

                    OnCloseCenterModal?.Invoke();

                    await Task.Delay(100).ContinueWith(_ =>
                    {
                        if (_centerModals.Count > 0)
                            OnShowCenterModal?.Invoke(_centerModals.Peek());
                        else
                            OnCloseCenterModalContainer?.Invoke();
                    });
                    break;

                case ModalType.RightSide:
                    if (_sideModals.Count == 0)
                        return;

                    _sideModals.Pop();

                    OnCloseSideModal?.Invoke();

                    await Task.Delay(100).ContinueWith(_ =>
                    {
                        if (_sideModals.Count > 0)
                            OnShowSideModal?.Invoke(_sideModals.Peek());
                        else
                            OnCloseSideModalContainer?.Invoke();
                    });
                    break;

                default: 
                    break;
            }
        }

        public void CloseAll(ModalType type)
        {
            switch (type)
            {
                case ModalType.Center:
                    _centerModals.Clear();
                    OnCloseCenterModal?.Invoke();
                    OnCloseCenterModalContainer?.Invoke();
                    break;

                case ModalType.RightSide:
                    _sideModals.Clear();
                    OnCloseSideModal?.Invoke();
                    OnCloseSideModalContainer?.Invoke();
                    break;

                default:
                    break;
            }
        }
    }
}
