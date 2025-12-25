using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Auth;
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

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = _authService.CurrentUser;
            if (_authService.IsAuthenticated && user is not null)
            {

                var identity = new ClaimsIdentity(BuildClaims(user), "CookieAuth");
                var principal = new ClaimsPrincipal(identity);

                return new AuthenticationState(principal);
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        private static List<Claim> BuildClaims(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.GivenName, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new("Telephone", user.Telephone ?? string.Empty),
                new("StudyGroup", user.StudyGroup ?? string.Empty)
            };

            // важное для проверки ролей в авторизации
            foreach (var role in user.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));

            // просто для проверки активной роли, активная роль уже есть в Roles
            if (user.Role.HasValue)
                claims.Add(new Claim("CurrentRole", user.Role.Value.ToString()));

            return claims;
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
