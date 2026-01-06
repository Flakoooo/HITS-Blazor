using HITSBlazor.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.NewPassword
{
    [AllowAnonymous]
    [Route("/new-password")]
    public partial class NewPassword
    {
        private NewPasswordModel newPasswordModel = new();
        private bool isLoading;

        [Parameter]
        [SupplyParameterFromQuery(Name = "verification-code")]
        public string? VerificationCode { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        protected override void OnInitialized()
        {
            if (!string.IsNullOrWhiteSpace(VerificationCode) 
                && Guid.TryParse(VerificationCode, out Guid guid)
            ) newPasswordModel.Id = guid;
        }

        private async Task HandleResetPassword()
        {
            if (isLoading) return;

            isLoading = true;

            if (await AuthService.ResetPasswordAsync(newPasswordModel))
            {
                newPasswordModel = new NewPasswordModel();
                Navigation.NavigateTo("/login");
            }

            isLoading = false;
        }
    }
}
