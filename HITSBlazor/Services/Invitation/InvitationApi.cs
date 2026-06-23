using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Requests;
using HITSBlazor.Models.Users.Responses;
using HITSBlazor.Utils;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Json;

namespace HITSBlazor.Services.Invitation
{
    public class InvitationApi(
        IHttpClientFactory httpClientFactory,
        ILogger<InvitationApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _invitationPath = "/api/invitation";

        private const string GET_INVITATION = "GetInvitation";
        private const string SEND_INVITATIONS = "SendInvitations";

        public async Task<ServiceResponse<InvitationResponse>> GetInvitationInfoAsync(Guid invitationId)
        {
            string path = $"{_invitationPath}/{invitationId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var invitation = await response.Content.ReadFromJsonAsync<InvitationResponse>(Settings.BaseJsonOptions);
                    if (invitation is null || string.IsNullOrWhiteSpace(invitation.Email) || invitation.Code == Guid.Empty)
                    {
                        if (_logger.IsEnabled(LogLevel.Warning))
                            _logger.LogWarning(
                                "{Operation} failed: {StatusCode} - {ErrorMessage}",
                                GET_INVITATION, response.StatusCode, "Error when get invitation info"
                            );

                        return ServiceResponse<InvitationResponse>.Failure("Ошибка при получении данных приглашения");
                    }

                    return ServiceResponse<InvitationResponse>.Success(invitation);
                },
                operationName: GET_INVITATION
            );
        }

        public async Task<ServiceResponse<string>> SendInvitationsAsync(InviteUsersRequest request)
        {
            var content = SerializeData(request);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(_invitationPath, content),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        if (_logger.IsEnabled(LogLevel.Warning))
                            _logger.LogWarning(
                                "{Operation} failed: {StatusCode} - {ErrorMessage}",
                                SEND_INVITATIONS, response.StatusCode, "Error when send invitations"
                            );

                        return ServiceResponse<string>.Failure("Не удалось отправить приглашения", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: SEND_INVITATIONS
            );
        }

        protected override string GetErrorMessage(HttpStatusCode statusCode, string operationName)
        {
            return operationName switch
            {
                GET_INVITATION => statusCode switch
                {
                    HttpStatusCode.NotFound => "Приглашенный пользователь не найден",
                    _ => base.GetErrorMessage(statusCode, operationName)
                },
                SEND_INVITATIONS => statusCode switch
                {
                    HttpStatusCode.Forbidden => "Отказано в доступе",
                    _ => base.GetErrorMessage(statusCode, operationName)
                },
                _ => base.GetErrorMessage(statusCode, operationName)
            };
        }
    }
}
