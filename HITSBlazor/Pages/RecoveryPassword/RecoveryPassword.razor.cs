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

        [Inject]
        private GlobalNotificationService NotificationService { get; set; } = null!;

        private async Task HandleRecovery()
        {
            if (isLoading) return;

            isLoading = true;

            try
            {
                if (string.IsNullOrWhiteSpace(recoveryModel.Email))
                {
                    NotificationService.ShowError("Пожалуйста, укажите почту");
                    isLoading = false;
                    return;
                }

                var email = recoveryModel.Email.Trim();
                var atIndex = email.IndexOf('@');

                if (atIndex <= 0 || atIndex == email.Length - 1 || email.Count(c => c == '@') != 1)
                {
                    NotificationService.ShowError("Неверный формат почты");
                    isLoading = false;
                    return;
                }

                var result = await AuthService.RequestPasswordRecoveryAsync(recoveryModel.Email);
                if (result.IsSuccess)
                {
                    if (result.Message is not null) NotificationService.ShowSuccess(result.Message);

                    recoveryModel = new RecoveryModel();
                    Navigation.NavigateTo($"/new-password?verification-code={result.Response}");
                }
                else
                {
                    NotificationService.ShowError(
                        result.Message ?? "Вы указали неверную почту для восстановления пароля. Попробуйте снова."
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
