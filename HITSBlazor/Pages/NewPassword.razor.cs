using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages
{
    [AllowAnonymous]
    [Route("/new-password")]
    public partial class NewPassword
    {
        private ResetPasswordRequest resetRequest = new();
        private bool isLoading;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private NotificationService NotificationService { get; set; } = null!;

        private async Task HandleResetPassword()
        {
            if (isLoading) return;

            isLoading = true;

            try
            {
                bool codeIsEmpty = string.IsNullOrWhiteSpace(resetRequest.Code);
                bool passwordIsEmpty = string.IsNullOrWhiteSpace(resetRequest.Password);

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

                if (resetRequest.Password.Length < 8)
                {
                    NotificationService.ShowError("Длина пароля не может быть меньше 8 символов");
                    isLoading = false;
                    return;
                }

                var result = await AuthService.ResetPasswordAsync(resetRequest);

                if (result.IsSuccess)
                {
                    resetRequest = new ResetPasswordRequest();
                    NotificationService.ShowSuccess(result.Message);
                    Navigation.NavigateTo("/login", true);
                }
                else
                {
                    NotificationService.ShowError(
                        result.Message ?? "Вы указали неверные данные для входа. Попробуйте снова."
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
