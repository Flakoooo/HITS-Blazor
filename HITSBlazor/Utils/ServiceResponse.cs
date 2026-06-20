using System.Net;

namespace HITSBlazor.Utils
{
    public record class ServiceResponse<T>
    {
        public bool IsSuccess { get; }
        public T? Response { get; }
        public string? Message { get; }
        public HttpStatusCode StatusCode { get; }

        private ServiceResponse(bool isSuccess, T? response, string? message, HttpStatusCode statusCode)
        {
            IsSuccess = isSuccess;
            Response = response;
            Message = message;
            StatusCode = statusCode;
        }

        public static ServiceResponse<T> Success(
            T? response, string? message = null, HttpStatusCode statusCode = HttpStatusCode.OK
        ) => new(true, response, message, statusCode);

        public static ServiceResponse<T> Failure(
            string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest
        ) => new(false, default, message, statusCode);
    }
}
