using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HITSBlazor.Pages.Auth.Login
{
    [AllowAnonymous]
    [Route("/login")]
    public partial class Login
    {
        private LoginModel _loginModel = new();
        private bool _submitting;

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private NavigationService Navigation { get; set; } = null!;

        private readonly Dictionary<string, string> _errors = [];

        protected async override Task OnInitializedAsync()
        {
            //TODOO: УБРАТЬ ПОСЛЕ РАЗРАБОТКИ
#if DEBUG
            _loginModel.Email = "kirill.vlasov.05@inbox.ru";
            _loginModel.Password = "12345678";
#else
            loginModel.Email = "lexunok@gmail.com";
            loginModel.Password = "lexunok2505";
#endif
        }

        //TODOO: обновить валидацию при добавлении API
        private async Task HandleLogin()
        {
            if (_submitting) return;
            _errors.Clear();

            _submitting = true;

            if (string.IsNullOrWhiteSpace(_loginModel.Email))
                _errors.Add("login", "Пожалуйста, заполните логин");

            if (string.IsNullOrWhiteSpace(_loginModel.Password))
                _errors.Add("password", "Заполните пароль");

            if (_errors.Count > 0) return;

            if (await AuthService.LoginAsync(_loginModel))
            {
                _loginModel = new LoginModel();
                await Navigation.NavigateToAsync("redirect");
            }

            _submitting = false;
        }
    }
}
