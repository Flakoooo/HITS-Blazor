using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Login
{
    [AllowAnonymous]
    [Route("/login")]
    public partial class Login
    {
        private LoginModel loginModel = new();
        private bool isLoading;

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [Inject]
        private GlobalNotificationService NotificationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            //TODO: УБРАТЬ ПОСЛЕ РАЗРАБОТКИ
            loginModel.Email = "lexunok@gmail.com";
            loginModel.Password = "lexunok2505";

            if (AuthService.IsAuthenticated)
                Navigation.NavigateTo("/");
        }

        private async Task HandleLogin()
        {
            if (isLoading) return;

            isLoading = true;

            try
            {
                bool emailIsEmpty = string.IsNullOrWhiteSpace(loginModel.Email);
                bool passwordIsEmpty = string.IsNullOrWhiteSpace(loginModel.Password);

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

                var email = loginModel.Email.Trim();
                var atIndex = email.IndexOf('@');

                if (atIndex <= 0 || atIndex == email.Length - 1 || email.Count(c => c == '@') != 1)
                {
                    NotificationService.ShowError("Неверный формат почты");
                    isLoading = false;
                    return;
                }

                if (loginModel.Password.Length < 8)
                {
                    NotificationService.ShowError("Длина пароля не может быть меньше 8 символов");
                    isLoading = false;
                    return;
                }

                var result = await AuthService.LoginAsync(loginModel);
                if (result.IsSuccess)
                {
                    loginModel = new LoginModel();
                    Navigation.NavigateTo("/");
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
