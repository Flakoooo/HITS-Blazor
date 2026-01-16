using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Services.Modal
{
    public class ModalService
    {
        public event Action<ModalData>? OnShow;
        public event Action? OnClose;

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

            OnShow?.Invoke(modalData);
        }

        public void Close()
        {
            OnClose?.Invoke();
        }
    }
}
