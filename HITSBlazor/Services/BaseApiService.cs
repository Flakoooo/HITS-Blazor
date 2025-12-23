using HITSBlazor.Utils;
using System.Net;

namespace HITSBlazor.Services
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
        protected async Task<ServiceResponse<T>> ExecuteApiCallAsync<T>(
            Func<Task<HttpResponseMessage>> apiCall,
            Func<HttpResponseMessage, Task<ServiceResponse<T>>> successHandler,
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

        private async Task<ServiceResponse<T>> HandleErrorResponse<T>(
            HttpResponseMessage response, string operationName)
        {
            var statusCode = response.StatusCode;
            var errorMessage = GetErrorMessage(statusCode, operationName);

            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(
                    "{Operation} failed: {StatusCode} - {ErrorMessage}",
                    operationName, statusCode, errorMessage
                );

            return ServiceResponse<T>.Failure(errorMessage, statusCode);
        }

        private ServiceResponse<T> HandleNetworkError<T>(HttpRequestException ex, string operationName)
        {
            if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, "Network error during {Operation}", operationName);

            return ServiceResponse<T>.Failure(
                "Нет соединения с сервером. Проверьте интернет.",
                HttpStatusCode.ServiceUnavailable
            );
        }

        private ServiceResponse<T> HandleTimeoutError<T>(TaskCanceledException ex, string operationName)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(ex, "{Operation} request timeout", operationName);

            return ServiceResponse<T>.Failure(
                "Превышено время ожидания ответа",
                HttpStatusCode.RequestTimeout
            );
        }

        private ServiceResponse<T> HandleUnexpectedError<T>(Exception ex, string operationName)
        {
            if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, "Unexpected error in {Operation}", operationName);

            return ServiceResponse<T>.Failure(
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
