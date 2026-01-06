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

        protected override void OnInitialized()
        {
            //TODO: УБРАТЬ ПОСЛЕ РАЗРАБОТКИ
#if DEBUG
            loginModel.Email = "kirill.vlasov.05@inbox.ru";
            loginModel.Password = "12345678";
#else
            loginModel.Email = "lexunok@gmail.com";
            loginModel.Password = "lexunok2505";
#endif

            if (AuthService.IsAuthenticated)
                Navigation.NavigateTo("/redirect");
        }

        private async Task HandleLogin()
        {
            if (isLoading) return;

            isLoading = true;

            if (await AuthService.LoginAsync(loginModel))
            {
                loginModel = new LoginModel();
                Navigation.NavigateTo("/redirect");
            }

            isLoading = false;
        }
    }
}
