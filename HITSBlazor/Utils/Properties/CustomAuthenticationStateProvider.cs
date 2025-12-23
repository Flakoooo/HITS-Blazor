using HITSBlazor.Services.Service.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace HITSBlazor.Utils.Properties
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthService _authService;

        public CustomAuthenticationStateProvider(IAuthService authService)
        {
            _authService = authService;
            _authService.OnAuthStateChanged += AuthStateChanged;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_authService.IsAuthenticated)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, "Authenticated User"),
                    new Claim(ClaimTypes.Role, "User")
                };

                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);

                return Task.FromResult(new AuthenticationState(principal));
            }

            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }

        private void AuthStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void Dispose()
        {
            _authService.OnAuthStateChanged -= AuthStateChanged;
        }
    }
}
