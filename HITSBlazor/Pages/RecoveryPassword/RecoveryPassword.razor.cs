using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.RecoveryPassword
{
    [AllowAnonymous]
    [Route("/recovery-password")]
    public partial class RecoveryPassword
    {
        private RecoveryModel recoveryModel = new();
        private bool isLoading;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        private async Task HandleRecovery()
        {
            if (isLoading) return;

            isLoading = true;

            Guid? result = await AuthService.RequestPasswordRecoveryAsync(recoveryModel);
            if (result is not null)
            {
                recoveryModel = new RecoveryModel();
                Navigation.NavigateTo($"/new-password?verification-code={result}");
            }

            isLoading = false;
        }
    }
}
