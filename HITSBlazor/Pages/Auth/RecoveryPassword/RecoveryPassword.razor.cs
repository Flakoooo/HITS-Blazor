using HITSBlazor.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HITSBlazor.Pages.Auth.RecoveryPassword
{
    [AllowAnonymous]
    [Route("/recovery-password")]
    public partial class RecoveryPassword
    {
        private RecoveryModel _recoveryModel = new();
        private bool _submitting;

        private readonly Dictionary<string, string> _errors = [];

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        private async Task HandleRecovery()
        {
            if (_submitting) return;
            _errors.Clear();

            _submitting = true;

            if (string.IsNullOrWhiteSpace(_recoveryModel.Email))
                _errors.Add("email", "Пожалуйста, укажите вашу почту");

            if (_errors.Count > 0) return;

            var result = await AuthService.RequestPasswordRecoveryAsync(_recoveryModel);
            if (result.HasValue)
            {
                _recoveryModel = new RecoveryModel();
                Navigation.NavigateTo($"/new-password?verification-code={result.Value}");
            }

            _submitting = false;
        }
    }
}
