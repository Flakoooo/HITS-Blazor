using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace HITSBlazor.Pages
{
    [AllowAnonymous]
    [Route("/recovery-password")]
    public partial class RecoveryPassword
    {
        private readonly RecoveryRequest recoveryRequest = new();
        private bool isLoading;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [Inject]
        private AuthService AuthService { get; set; } = null!;

        [Inject]
        private NotificationService NotificationService { get; set; } = null!;

        private async Task HandleRecovery()
        {
            if (isLoading) return;

            isLoading = true;

            try
            {
                // Ручная валидация (как на странице логина)
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(recoveryRequest);

                bool isValid = Validator.TryValidateObject(
                    recoveryRequest, validationContext, validationResults, true);

                if (!isValid)
                {
                    NotificationService.ShowError("Введите корректный email адрес");
                    isLoading = false;
                    return;
                }

                // Имитация вызова API (заменить на реальный)
                await Task.Delay(1500);

                // Проверка существования email (мок)
                var mockUsers = new[] { "user@example.com", "admin@example.com" };
                if (!mockUsers.Contains(recoveryRequest.Email.ToLower()))
                {
                    NotificationService.ShowError("Пользователь с таким email не найден");
                    isLoading = false;
                    return;
                }

                // Автоматический редирект через 5 секунд
                await Task.Delay(5000);
                Navigation.NavigateTo("/login");
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
