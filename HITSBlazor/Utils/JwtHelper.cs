using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Text.Json;

namespace HITSBlazor.Utils
{
    public static class JwtHelper
    {
        public static User? DecodeJwtPayload(string jwt)
        {
            var handler = new JsonWebTokenHandler();

            if (!handler.CanReadToken(jwt)) return null;

            var token = handler.ReadJsonWebToken(jwt);
            var claims = token.Claims.ToDictionary(c => c.Type, c => c.Value);

            List<RoleType> roles = [];
            if (claims.TryGetValue("roles", out var rolesValue))
            {
                var roleStrings = JsonSerializer.Deserialize<string[]>(rolesValue);
                if (roleStrings != null)
                    foreach (var roleStr in roleStrings)
                        if (Enum.TryParse<RoleType>(roleStr, out var role))
                            roles.Add(role);
            }

            Guid userId;
            if (claims.TryGetValue("sub", out var sub) && Guid.TryParse(sub, out var guid))
                userId = guid;
            else 
                return null;

            return new User
            {
                Id = userId,
                Email = claims.GetValueOrDefault("email") ?? "",
                FirstName = claims.GetValueOrDefault("first_name") ?? "",
                LastName = claims.GetValueOrDefault("last_name") ?? "",
                Roles = roles
            };
        }

        public static bool IsTokenExpired(string? jwt)
        {
            if (string.IsNullOrEmpty(jwt))
                return true;

            try
            {
                var handler = new JsonWebTokenHandler();
                if (!handler.CanReadToken(jwt))
                    return true;

                var token = handler.ReadJsonWebToken(jwt);

                var exp = token.ValidTo;
                var now = DateTime.UtcNow;

                return exp < now;
            }
            catch
            {
                return true;
            }
        }
    }
}
