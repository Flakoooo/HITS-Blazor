using HITSBlazor.Services;
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

        [Inject]
        private NotificationService NotificationService { get; set; } = null!;

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

            try
            {
                bool codeIsEmpty = string.IsNullOrWhiteSpace(newPasswordModel.Code);
                bool passwordIsEmpty = string.IsNullOrWhiteSpace(newPasswordModel.Password);

                if (codeIsEmpty || passwordIsEmpty)
                {
                    var errorMessage = "";

                    if (codeIsEmpty && passwordIsEmpty)
                        errorMessage = "Пожалуйста, введите код и новый пароль";
                    else if (codeIsEmpty)
                        errorMessage = "Пожалуйста, введите код";
                    else
                        errorMessage = "Пожалуйста, введите новый пароль";

                    NotificationService.ShowError(errorMessage);
                    isLoading = false;
                    return;
                }

                if (newPasswordModel.Password.Length < 8)
                {
                    NotificationService.ShowError("Длина пароля не может быть меньше 8 символов");
                    isLoading = false;
                    return;
                }

                var result = await AuthService.ResetPasswordAsync(newPasswordModel);

                if (result.IsSuccess)
                {
                    if (result.Message is not null) NotificationService.ShowSuccess(result.Message);

                    newPasswordModel = new NewPasswordModel();
                    Navigation.NavigateTo("/login", true);
                }
                else
                {
                    NotificationService.ShowError(
                        result.Message ?? "Вы указали неверные данные для сброса. Попробуйте снова."
                    );
                }
            }
            catch (Exception ex)
            {
                NotificationService.ShowError($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}
