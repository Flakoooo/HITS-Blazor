using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.CenterModals.SelectActiveRoleModal
{
    public partial class SelectActiveRoleModal
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        public List<RoleType> Roles { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            Roles = AuthService.CurrentUser?.Roles ?? [];
        }

        private async void SetUserRole(RoleType role)
        {
            await AuthService.SetUserRoleAsync(role);
            ModalService.Close(ModalType.Center);
        }
    }
}
