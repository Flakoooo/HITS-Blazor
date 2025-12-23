using HITSBlazor.Utils;
using System.Net;

namespace HITSBlazor.Services.Api
{
    public class BaseApiService
    {
        protected readonly HttpClient _httpClient;
        protected readonly ILogger _logger;

        protected BaseApiService(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        protected async Task<ApiResponse<T>> ExecuteApiCallAsync<T>(
            Func<Task<HttpResponseMessage>> apiCall,
            Func<HttpResponseMessage, Task<ApiResponse<T>>> successHandler,
            string operationName)
        {
            try
            {
                if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("Executing {Operation}", operationName);

                var response = await apiCall();

                if (response.IsSuccessStatusCode)
                    return await successHandler(response);

                return await HandleErrorResponse<T>(response, operationName);
            }
            catch (HttpRequestException ex)
            {
                return HandleNetworkError<T>(ex, operationName);
            }
            catch (TaskCanceledException ex)
            {
                return HandleTimeoutError<T>(ex, operationName);
            }
            catch (Exception ex)
            {
                return HandleUnexpectedError<T>(ex, operationName);
            }
        }

        private async Task<ApiResponse<T>> HandleErrorResponse<T>(
            HttpResponseMessage response, string operationName)
        {
            var statusCode = response.StatusCode;
            var errorMessage = GetErrorMessage(statusCode, operationName);

            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(
                    "{Operation} failed: {StatusCode} - {ErrorMessage}",
                    operationName, statusCode, errorMessage
                );

            return ApiResponse<T>.Failure(errorMessage, statusCode);
        }

        private ApiResponse<T> HandleNetworkError<T>(HttpRequestException ex, string operationName)
        {
            if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, "Network error during {Operation}", operationName);

            return ApiResponse<T>.Failure(
                "Нет соединения с сервером. Проверьте интернет.",
                HttpStatusCode.ServiceUnavailable
            );
        }

        private ApiResponse<T> HandleTimeoutError<T>(TaskCanceledException ex, string operationName)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(ex, "{Operation} request timeout", operationName);

            return ApiResponse<T>.Failure(
                "Превышено время ожидания ответа",
                HttpStatusCode.RequestTimeout
            );
        }

        private ApiResponse<T> HandleUnexpectedError<T>(Exception ex, string operationName)
        {
            if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, "Unexpected error in {Operation}", operationName);

            return ApiResponse<T>.Failure(
                "Внутренняя ошибка приложения",
                HttpStatusCode.InternalServerError
            );
        }

        protected virtual string GetErrorMessage(HttpStatusCode statusCode, string operationName)
        {
            return statusCode switch
            {
                HttpStatusCode.Unauthorized => "Неавторизованный доступ",
                HttpStatusCode.Forbidden => "Доступ запрещен",
                HttpStatusCode.NotFound => "Ресурс не найден",
                HttpStatusCode.BadRequest => "Неверный запрос",
                HttpStatusCode.InternalServerError => "Ошибка сервера",
                _ => $"Ошибка: {statusCode}"
            };
        }
    }
}
