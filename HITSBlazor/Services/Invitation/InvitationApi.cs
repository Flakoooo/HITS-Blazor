using HITSBlazor.Utils;
using Newtonsoft.Json.Linq;
using System.Net;

namespace HITSBlazor.Services.Invitation
{
    public class InvitationApi(
        IHttpClientFactory httpClientFactory,
        ILogger<InvitationApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _invitationPath = "/api/invitation";

        private const string GET_EMAIL = "GetInvitation";

        public Task<ServiceResponse<string>> GetEmailByInvitationId(Guid invitationId) => ExecuteApiCallAsync(
            apiCall: () => _httpClient.GetAsync($"{_invitationPath}/{invitationId}"),
            successHandler: async response =>
            {
                var content = await response.Content.ReadAsStringAsync();
                var email = JObject.Parse(content)["email"]?.ToString();

                if (email == null)
                    return ServiceResponse<string>.Failure("Ошибка при получении почты");

                return ServiceResponse<string>.Success(email);
            },
            operationName: GET_EMAIL
        );

        protected override string GetErrorMessage(HttpStatusCode statusCode, string operationName)
        {
            return operationName switch
            {
                GET_EMAIL => statusCode switch
                {
                    HttpStatusCode.NotFound => "Приглашенный пользователь не найден",
                    _ => base.GetErrorMessage(statusCode, operationName)
                },
                _ => base.GetErrorMessage(statusCode, operationName)
            };
        }
    }
}
