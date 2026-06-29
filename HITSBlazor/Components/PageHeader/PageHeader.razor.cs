using HITSBlazor.Components.Modals.RightSideModals.NotificationsModal;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Profiles;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.PageHeader
{
    public partial class PageHeader
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private IProfileService ProfileService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private string? _userAvatar = null;
        private User? CurrentUser { get; set; } = null;
        private RoleType? CurrentRole { get; set; } = null;

        protected override async Task OnInitializedAsync()
        {
            AuthService.OnAuthStateChanged += AuthStateChanged;
            AuthService.OnActiveRoleChanged += RoleStateChanged;

            ProfileService.OnUserAvatarHasChanged += UserAvatarHasChanged;

            CurrentUser = AuthService.CurrentUser;

            if (CurrentUser is not null)
                _userAvatar = await ProfileService.GetUserProifleAvatarAsync(CurrentUser.Id);
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

        private void UserAvatarHasChanged(string? newAvatar)
        {
            _userAvatar = newAvatar;
            StateHasChanged();
        }

        public void Dispose()
        {
            AuthService.OnAuthStateChanged -= AuthStateChanged;
            AuthService.OnActiveRoleChanged -= RoleStateChanged;

            ProfileService.OnUserAvatarHasChanged -= UserAvatarHasChanged;
        }
    }
}
