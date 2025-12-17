using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace HITSBlazor.Pages
{
    [AllowAnonymous]
    [Route("/new-password")]
    public partial class NewPassword
    {
        private ResetPasswordRequest resetRequest = new();
        private bool isLoading;
        private string? errorMessage;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [Inject]
        private NotificationService NotificationService { get; set; } = null!;

        private string? ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                StateHasChanged();
            }
        }

        private async Task HandleResetPassword()
        {
            if (isLoading) return;

            isLoading = true;
            ErrorMessage = null;

            try
            {
                // Валидация
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(resetRequest);

                bool isValid = Validator.TryValidateObject(
                    resetRequest, validationContext, validationResults, true);

                if (!isValid)
                {
                    var firstError = validationResults.FirstOrDefault()?.ErrorMessage;
                    NotificationService.ShowError(firstError ?? "Пожалуйста, заполните все поля корректно.");
                    isLoading = false;
                    return;
                }

                // Проверка минимальной длины пароля
                if (resetRequest.Password.Length < 8)
                {
                    NotificationService.ShowError("Пароль должен состоять как минимум из 8 символов");
                    isLoading = false;
                    return;
                }

                // Проверка кода (простая мок-проверка)
                if (resetRequest.Code != "123456") // Заменить на реальную проверку
                {
                    NotificationService.ShowError("Неверно введен код");
                    isLoading = false;
                    return;
                }

                // Имитация отправки запроса
                await Task.Delay(1500);

                // Редирект на страницу логина через 2 секунды
                await Task.Delay(2000);
                NotificationService.ShowSuccess("Пароль успешно изменен!");
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

        private void GoBack()
        {
            Navigation.NavigateTo("/recovery-password");
        }
    }
}
