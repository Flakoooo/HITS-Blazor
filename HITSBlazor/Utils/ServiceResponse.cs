using System.Net;

namespace HITSBlazor.Utils
{
    public record class ServiceResponse<T>(
        bool IsSuccess,
        T? Response,
        string? Error,
        HttpStatusCode StatusCode = HttpStatusCode.BadRequest
    )
    {
        public bool IsSuccess { get; } = IsSuccess;
        public T? Response { get; } = Response;
        public string? Message { get; } = Error;
        public HttpStatusCode StatusCode { get; } = StatusCode;

        public static ServiceResponse<T> Success(
            T? response, string? message = null, HttpStatusCode statusCode = HttpStatusCode.OK
        ) => new(true, response, message, statusCode);

        public static ServiceResponse<T> Failure(
            string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest
        ) => new(false, default, message, statusCode);
    }
}
