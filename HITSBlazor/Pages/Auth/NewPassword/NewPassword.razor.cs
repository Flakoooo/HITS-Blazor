using HITSBlazor.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Auth.NewPassword
{
    [AllowAnonymous]
    [Route("/new-password")]
    public partial class NewPassword
    {
        private NewPasswordModel _newPasswordModel = new();
        private bool _submitting;

        [Parameter]
        [SupplyParameterFromQuery(Name = "verification-code")]
        public string? VerificationCode { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        private readonly Dictionary<string, string> _errors = [];

        protected override void OnInitialized()
        {
            if (!string.IsNullOrWhiteSpace(VerificationCode) 
                && Guid.TryParse(VerificationCode, out Guid guid)
            ) _newPasswordModel.Id = guid;
        }

        //TODOO: обновить валидацию при добавлении API
        private async Task HandleResetPassword()
        {
            if (_submitting) return;
            _errors.Clear();

            _submitting = true;

            if (string.IsNullOrWhiteSpace(_newPasswordModel.Code))
                _errors.Add("code", "Пожалуйста, укажите код");

            if (string.IsNullOrWhiteSpace(_newPasswordModel.Password))
                _errors.Add("password", "Пожалуйста, укажите новый пароль");

            if (_errors.Count > 0) return;

            if (await AuthService.ResetPasswordAsync(_newPasswordModel))
            {
                _newPasswordModel = new NewPasswordModel();
                Navigation.NavigateTo("/login");
            }

            _submitting = false;
        }
    }
}
