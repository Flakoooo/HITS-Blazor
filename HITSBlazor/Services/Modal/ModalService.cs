using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Services.Modal
{
    public class ModalService
    {
        public event Action<ModalData>? OnShow;
        public event Action? OnClose;
        public event Action? OnCloseContainer;

        private Stack<ModalData> _modalStack = [];

        public int ModalCount => _modalStack.Count;
        public bool HasModals => _modalStack.Count > 0;
        public ModalData? CurrentModal => _modalStack.Count > 0 ? _modalStack.Peek() : null;

        public void Show<TComponent>(
            ModalType type = ModalType.Center,
            bool blockCloseModal = false, 
            Dictionary<string, object>? parameters = null,
            string? customClass = null
        ) where TComponent : ComponentBase
        {
            Show(typeof(TComponent), type, blockCloseModal, parameters, customClass);
        }

        public void Show(
            Type componentType,
            ModalType type = ModalType.Center,
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

            _modalStack.Push(modalData);
            OnShow?.Invoke(modalData);
        }

        public async void Close()
        {
            if (_modalStack.Count == 0)
                return;

            _modalStack.Pop();

            OnClose?.Invoke();

            await Task.Delay(100).ContinueWith(_ =>
            {
                if (_modalStack.Count > 0)
                    OnShow?.Invoke(_modalStack.Peek());
                else
                    OnCloseContainer?.Invoke();
            });
        }

        public void CloseAll()
        {
            _modalStack.Clear();
            OnClose?.Invoke();
            OnCloseContainer?.Invoke();
        }
    }
}
