using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages
{
    [AllowAnonymous]
    [Route("/recovery-password")]
    public partial class RecoveryPassword
    {
        private RecoveryRequest recoveryRequest = new();
        private bool isLoading;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private NotificationService NotificationService { get; set; } = null!;

        private async Task HandleRecovery()
        {
            if (isLoading) return;

            isLoading = true;

            try
            {
                if (string.IsNullOrWhiteSpace(recoveryRequest.Email))
                {
                    NotificationService.ShowError("Пожалуйста, укажите почту");
                    isLoading = false;
                    return;
                }

                var email = recoveryRequest.Email.Trim();
                var atIndex = email.IndexOf('@');

                if (atIndex <= 0 || atIndex == email.Length - 1 || email.Count(c => c == '@') != 1)
                {
                    NotificationService.ShowError("Неверный формат почты");
                    isLoading = false;
                    return;
                }

                var result = await AuthService.RequestPasswordRecoveryAsync(recoveryRequest.Email);

                if (result.IsSuccess)
                {
                    NotificationService.ShowSuccess(result.Message);

                    recoveryRequest = new RecoveryRequest();
                    Navigation.NavigateTo("/new-password");
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
