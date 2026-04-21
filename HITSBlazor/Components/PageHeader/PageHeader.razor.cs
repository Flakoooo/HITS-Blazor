using HITSBlazor.Components.Modals.RightSideModals.NotificationsModal;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.PageHeader
{
    public partial class PageHeader
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private User? CurrentUser { get; set; } = null;
        private RoleType? CurrentRole { get; set; } = null;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                AuthService.OnAuthStateChanged += AuthStateChanged;
                AuthService.OnActiveRoleChanged += RoleStateChanged;
                CurrentUser = AuthService.CurrentUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка инициализации MainLayout: {ex.Message}");
            }
        }

        private void AuthStateChanged()
        {
            CurrentUser = AuthService.CurrentUser;
            StateHasChanged();
        }

        private void RoleStateChanged(RoleType? role)
        {
            CurrentRole = role;
            StateHasChanged();
        }

        private void ShowUserProfile()
        {
            var userId = CurrentUser?.Id;
            if (userId is null) return;

            ModalService.ShowProfileModal(userId.Value);
        }

        private void ShowRoleModal() => ModalService.ShowActiveRoleModal();

        private void ShowNotificationModal()
        {
            ModalService.Show<NotificationsModal>(ModalType.RightSide);
        }

        public void Dispose()
        {
            AuthService.OnAuthStateChanged -= AuthStateChanged;
            AuthService.OnActiveRoleChanged -= RoleStateChanged;
        }
    }
}
