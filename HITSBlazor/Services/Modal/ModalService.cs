using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.CenterModals.ConfirmModal;
using HITSBlazor.Components.Modals.CenterModals.SelectActiveRoleModal;
using HITSBlazor.Components.Modals.CenterModals.TaskModal;
using HITSBlazor.Components.Modals.RightSideModals.IdeaModal;
using HITSBlazor.Components.Modals.RightSideModals.ProfileModal;
using HITSBlazor.Components.Modals.RightSideModals.TeamModal;
using HITSBlazor.Components.Typography;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Services.Modal
{
    public class ModalService
    {
        public event Action? OnCenterModalsUpdated;

        public event Action<ModalData>? OnShowSideModal;

        public event Action? OnCloseSideModal;

        public event Action? OnCloseSideModalContainer;

        public List<ModalData> CenterModals { get; private set; } = [];
        public Stack<ModalData> SideModals { get; private set; } = [];

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
                    CenterModals.Add(modalData);
                    OnCenterModalsUpdated?.Invoke();
                    break;

                case ModalType.RightSide:
                    SideModals.Push(modalData);
                    OnShowSideModal?.Invoke(modalData);
                    break;

                default:
                    break;
            }
        }

        public async Task Close(ModalType type)
        {
            switch (type)
            {
                case ModalType.Center:
                    if (CenterModals.Count == 0)
                        return;

                    CenterModals.Last().State = ModalState.Leave;
                    OnCenterModalsUpdated?.Invoke();
                    await Task.Delay(100);

                    CenterModals.Remove(CenterModals.Last());
                    OnCenterModalsUpdated?.Invoke();
                    break;

                case ModalType.RightSide:
                    if (SideModals.Count == 0)
                        return;

                    SideModals.Pop();

                    OnCloseSideModal?.Invoke();

                    await Task.Delay(100).ContinueWith(_ =>
                    {
                        if (SideModals.Count > 0)
                            OnShowSideModal?.Invoke(SideModals.Peek());
                        else
                            OnCloseSideModalContainer?.Invoke();
                    });
                    break;

                default: 
                    break;
            }
        }

        public async Task CloseAll(ModalType type)
        {
            switch (type)
            {
                case ModalType.Center:
                    for (int i = CenterModals.Count - 1; i >= 0; i--)
                        CenterModals[i].State = ModalState.Leave;

                    OnCenterModalsUpdated?.Invoke();

                    await Task.Delay(100);

                    CenterModals.Clear();
                    OnCenterModalsUpdated?.Invoke();
                    break;

                case ModalType.RightSide:
                    SideModals.Clear();
                    OnCloseSideModal?.Invoke();
                    OnCloseSideModalContainer?.Invoke();
                    break;

                default:
                    break;
            }
        }

        public void ShowActiveRoleModal() => Show<SelectActiveRoleModal>(
            ModalType.Center, 
            blockCloseModal: true
        );

        public void ShowConfirmModal(
            string questionText,
            Func<Task> confirmMethod,
            int? questionTextFontSize = null,
            TextColor? questionTextColor = null,
            string? questionTextCustomClass = null,
            ButtonVariant? cancelButtonVariant = null,
            string? cancelButtonText = null,
            ButtonVariant? confirmButtonVariant = null,
            string? confirmButtonText = null
        )
        {
            var parameters = new Dictionary<string, object>();

            if (questionTextFontSize.HasValue) 
                parameters.Add(nameof(ConfirmModal.QuestionTextFontSize), questionTextFontSize.Value);

            if (questionTextColor.HasValue)
                parameters.Add(nameof(ConfirmModal.QuestionTextColor), questionTextColor.Value);

            if (!string.IsNullOrWhiteSpace(questionTextCustomClass))
                parameters.Add(nameof(ConfirmModal.QuestionTextCustomClass), questionTextCustomClass);

            parameters.Add(nameof(ConfirmModal.QuestionText), questionText);

            if (cancelButtonVariant.HasValue)
                parameters.Add(nameof(ConfirmModal.CancelButtonVariant), cancelButtonVariant.Value);

            if (!string.IsNullOrWhiteSpace(cancelButtonText))
                parameters.Add(nameof(ConfirmModal.CancelButtonText), cancelButtonText);

            if (confirmButtonVariant.HasValue)
                parameters.Add(nameof(ConfirmModal.ConfirmButtonVariant), confirmButtonVariant.Value);

            if (!string.IsNullOrWhiteSpace(confirmButtonText))
                parameters.Add(nameof(ConfirmModal.ConfirmButtonText), confirmButtonText);

            parameters.Add(nameof(ConfirmModal.ConfirmMethod), confirmMethod);

            Show<ConfirmModal>(type: ModalType.Center, parameters: parameters);
        }

        public void ShowProfileModal(Guid userId) => Show<ProfileModal>(
            ModalType.RightSide,
            parameters: new Dictionary<string, object> { [nameof(ProfileModal.UserId)] = userId }
        );

        public void ShowIdeaModal(Guid ideaId) => Show<IdeaModal>(
            ModalType.RightSide,
            parameters: new Dictionary<string, object> { [nameof(IdeaModal.IdeaId)] = ideaId }
        );

        public void ShowTeamModal(Guid teamId) => Show<TeamModal>(
            type: ModalType.RightSide,
            parameters: new Dictionary<string, object> { [nameof(TeamModal.TeamId)] = teamId }
        );

        public void ShowTaskModal() => Show<TaskModal>(ModalType.Center);
    }
}
