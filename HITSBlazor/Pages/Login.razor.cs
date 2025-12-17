using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages
{
    [AllowAnonymous]
    [Route("/login")]
    public partial class Login
    {
        private LoginRequest loginRequest = new();
        private bool isLoading;

        [Inject]
        private AuthService AuthService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [Inject]
        private NotificationService NotificationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            if (AuthService.IsAuthenticated)
            {
                Navigation.NavigateTo("/", true);
            }
        }

        private async Task HandleLogin()
        {
            if (isLoading) return;

            isLoading = true;

            try
            {
                // Ручная валидация перед отправкой
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(loginRequest);

                bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
                    loginRequest, validationContext, validationResults, true);

                if (!isValid)
                {
                    NotificationService.ShowError("Вы указали неверные данные для входа. Попробуйте снова.");
                    isLoading = false;
                    return;
                }

                // Если валидация прошла, отправляем запрос
                var result = await AuthService.LoginAsync(loginRequest);

                if (result.Success)
                {
                    loginRequest = new LoginRequest();
                    Navigation.NavigateTo("/", true);
                }
                else
                {
                    NotificationService.ShowError("Вы указали неверные данные для входа. Попробуйте снова.");
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
