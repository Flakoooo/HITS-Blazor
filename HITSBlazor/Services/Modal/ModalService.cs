using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Services.Modal
{
    public class ModalService
    {
        public event Action<ModalData>? OnShow;
        public event Action? OnClose;

        public void Show<TComponent>(bool blockCloseModal = false, Dictionary<string, object>? parameters = null) where TComponent : ComponentBase
        {
            var modalData = new ModalData
            {
                ComponentType = typeof(TComponent),
                BlockCloseModal = blockCloseModal,
                Parameters = parameters ?? []
            };

            OnShow?.Invoke(modalData);
        }

        public void Show(Type componentType, bool blockCloseModal = false, Dictionary<string, object>? parameters = null)
        {
            var modalData = new ModalData
            {
                ComponentType = componentType,
                BlockCloseModal = blockCloseModal,
                Parameters = parameters ?? []
            };

            OnShow?.Invoke(modalData);
        }

        public void Close()
        {
            OnClose?.Invoke();
        }
    }
}
