using HITSBlazor.Pages.Login;
using HITSBlazor.Pages.NewPassword;
using HITSBlazor.Pages.Register;
using HITSBlazor.Utils;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Json;

namespace HITSBlazor.Services.Auth
{
    public class AuthApi(
        IHttpClientFactory httpClientFactory, 
        ILogger<AuthApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _authPath = "/api/auth";

        private const string LOGIN_OPERATION = "Login";
        private const string LOGOUT_OPERATION = "Logout";
        private const string REFRESH_TOKEN_OPERATION = "RefreshToken";
        private const string PASSWORD_VERIFICATION_OPERATION = "PasswordVerification";
        private const string PASSWORD_NEW_OPERATION = "PasswordNew";
        private const string REGISTRATION_OPERATION = "Registration";

        public async Task<ServiceResponse<bool>> LoginAsync(LoginModel request) => await ExecuteApiCallAsync(
            apiCall: () => _httpClient.PostAsJsonAsync($"{_authPath}/login", request, Settings.BaseJsonOptions),
            successHandler: async response =>
            {
                if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("Login successful for user {Email}", request.Email);

                return ServiceResponse<bool>.Success(true);
            },
            operationName: LOGIN_OPERATION
        );

        public async Task<ServiceResponse<bool>> LogoutAsync() =>await ExecuteApiCallAsync(
            apiCall: () => _httpClient.PostAsync($"{_authPath}/logout", null),
            successHandler: async response =>
            {
                if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("Logout successful for user ");

                return ServiceResponse<bool>.Success(true);
            },
            operationName: LOGOUT_OPERATION
        );

        public async Task<ServiceResponse<bool>> RefreshTokenAsync() => await ExecuteApiCallAsync(
            apiCall: () => _httpClient.PostAsync($"{_authPath}/refresh", null),
            successHandler: async response =>
            {
                if (AppEnvironment.IsLogEnabled)
                    _logger.LogInformation("Token refresh successful");

                return ServiceResponse<bool>.Success(true);
            },
            operationName: REFRESH_TOKEN_OPERATION
        );

        public async Task<ServiceResponse<Guid>> PasswordVerificationAsync(string email) => await ExecuteApiCallAsync(
            apiCall: () => _httpClient.PostAsync($"{_authPath}/password/verification/{email}", null),
            successHandler: async response =>
            {
                string content = await response.Content.ReadAsStringAsync();
                string? strGuid = JObject.Parse(content)["id"]?.ToString();
                if (strGuid is null || !Guid.TryParse(strGuid, out Guid guid))
                {
                    if (_logger.IsEnabled(LogLevel.Warning))
                        _logger.LogWarning(
                            "{Operation} failed: {StatusCode} - {ErrorMessage}",
                            PASSWORD_VERIFICATION_OPERATION, response.StatusCode, "Error when parse GUID"
                        );

                    return ServiceResponse<Guid>.Failure("Не удалось подтвердить почту, попробуйте позже");
                }

                if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("Password verification successful for user {Email}", email);

                return ServiceResponse<Guid>.Success(guid, "Инструкции по восстановлению отправлены на email");
            },
            operationName: PASSWORD_VERIFICATION_OPERATION
        );

        public async Task<ServiceResponse<bool>> PasswordNewAsync(NewPasswordModel newPasswordModel) => await ExecuteApiCallAsync(
            apiCall: () => _httpClient.PutAsJsonAsync($"{_authPath}/password", newPasswordModel, Settings.BaseJsonOptions),
            successHandler: async response =>
            {
                string content = await response.Content.ReadAsStringAsync();
                string? message = JObject.Parse(content)["message"]?.ToString();

                if (AppEnvironment.IsLogEnabled)
                    _logger.LogInformation("New password successful");

                return ServiceResponse<bool>.Success(true, message);
            },
            operationName: PASSWORD_NEW_OPERATION
        );

        public async Task<ServiceResponse<bool>> RegistrationUserAsync(RegisterModel request, Guid invitationId) => await ExecuteApiCallAsync(
            apiCall: () => _httpClient.PostAsJsonAsync($"{_authPath}/registration/{invitationId}", request, Settings.BaseJsonOptions),
            successHandler: async response =>
            {
                if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("Registration successful for user {Email}", request.Email);

                return ServiceResponse<bool>.Success(true, "Успешная регистрация");
            },
            operationName: REGISTRATION_OPERATION
        );

        protected override string GetErrorMessage(HttpStatusCode statusCode, string operationName)
        {
            return operationName switch
            {
                LOGIN_OPERATION => statusCode switch
                {
                    HttpStatusCode.Unauthorized => "Неверная почта или пароль",
                    HttpStatusCode.UnprocessableEntity => "Некорректная почта или пароль",
                    HttpStatusCode.InternalServerError => "Произошла ошибка сервера, попробуйте позже",
                    _ => base.GetErrorMessage(statusCode, operationName)
                },
                LOGOUT_OPERATION => statusCode switch
                {
                    HttpStatusCode.Unauthorized => "Выход из аккаунта невозможен",
                    _ => base.GetErrorMessage(statusCode, operationName)
                },
                REFRESH_TOKEN_OPERATION => statusCode switch
                {
                    HttpStatusCode.Unauthorized => "Не удалось обновить токен. Требуется повторный вход",
                    _ => base.GetErrorMessage(statusCode, operationName)
                },
                REGISTRATION_OPERATION => statusCode switch
                {
                    HttpStatusCode.NotFound => "Не удалось найти приглашение",
                    HttpStatusCode.BadRequest => "Данная почта не совпадает с приглашением или уже используется",
                    HttpStatusCode.UnprocessableEntity => "Некорректный данные",
                    HttpStatusCode.InternalServerError => "Произошла ошибка сервера, попробуйте позже",
                    _ => base.GetErrorMessage(statusCode, operationName)
                },
                PASSWORD_VERIFICATION_OPERATION => statusCode switch 
                {
                    HttpStatusCode.NotFound => "Пользователь с такой почтой не найден",
                    _ => base.GetErrorMessage(statusCode, operationName)
                },
                _ => base.GetErrorMessage(statusCode, operationName)
            };
        }
    }
}
