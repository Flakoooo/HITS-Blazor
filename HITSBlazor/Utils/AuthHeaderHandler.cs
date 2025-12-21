using HITSBlazor.Services.Service.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;

namespace HITSBlazor.Utils
{
    public class AuthHeaderHandler(
        IAuthService authService,
        NavigationManager navigationManager,
        ILogger<AuthHeaderHandler> logger
    ) : DelegatingHandler
    {
        private readonly IAuthService _authService = authService;
        private readonly NavigationManager _navigationManager = navigationManager;
        private readonly ILogger<AuthHeaderHandler> _logger = logger;

        private bool _isRefreshingToken;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = await _authService.GetAccessTokenAsync();
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized && !_isRefreshingToken)
            {
                _isRefreshingToken = true;

                try
                {
                    _logger.LogInformation("Access token invalid, attempting refresh...");

                    var newToken = await _authService.GetAccessTokenAsync(forceRefresh: true);

                    if (!string.IsNullOrEmpty(newToken) && newToken != token)
                    {
                        _logger.LogInformation("Token refreshed successfully, retrying request");

                        // Повторяем запрос с новым токеном
                        request.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", newToken);
                        return await base.SendAsync(request, cancellationToken);
                    }
                    else
                    {
                        _logger.LogWarning("Token refresh failed, redirecting to login");

                        // Если обновить не удалось - редирект на логин
                        _navigationManager.NavigateTo("/login", forceLoad: true);
                    }
                }
                finally
                {
                    _isRefreshingToken = false;
                }
            }

            return response;
        }
    }
}
