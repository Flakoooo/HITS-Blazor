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
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [Inject]
        private NotificationService NotificationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            if (AuthService.IsAuthenticated)
                Navigation.NavigateTo("/", true);
        }

        private async Task HandleLogin()
        {
            if (isLoading) return;

            isLoading = true;

            try
            {
                bool emailIsEmpty = string.IsNullOrWhiteSpace(loginRequest.Email);
                bool passwordIsEmpty = string.IsNullOrWhiteSpace(loginRequest.Password);

                if (emailIsEmpty || passwordIsEmpty)
                {
                    var errorMessage = "";

                    if (emailIsEmpty && passwordIsEmpty)
                        errorMessage = "Пожалуйста, заполните логин и пароль";
                    else if (emailIsEmpty)
                        errorMessage = "Пожалуйста, заполните логин";
                    else
                        errorMessage = "Пожалуйста, заполните пароль";

                    NotificationService.ShowError(errorMessage);
                    isLoading = false;
                    return;
                }

                var email = loginRequest.Email.Trim();
                var atIndex = email.IndexOf('@');

                if (atIndex <= 0 || atIndex == email.Length - 1 || email.Count(c => c == '@') != 1)
                {
                    NotificationService.ShowError("Неверный формат почты");
                    isLoading = false;
                    return;
                }

                if (loginRequest.Password.Length < 8)
                {
                    NotificationService.ShowError("Длина пароля не может быть меньше 8 символов");
                    isLoading = false;
                    return;
                }

                var result = await AuthService.LoginAsync(loginRequest);

                if (result.IsSuccess)
                {
                    loginRequest = new LoginRequest();
                    Navigation.NavigateTo("/", true);
                }
                else
                {
                    NotificationService.ShowError(
                        result.ErrorMessage ?? "Вы указали неверные данные для входа. Попробуйте снова."
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
