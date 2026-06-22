using HITSBlazor.Models.Users.Requests;
using static HITSBlazor.Utils.Mocks.Common.MockInvitation;

namespace HITSBlazor.Services.Invitation
{
    public class InvitationService(
        InvitationApi invitationApi, 
        ILogger<InvitationService> logger,
        GlobalNotificationService globalNotificationService
    ) : IInvitationService
    {
        private readonly InvitationApi _invitationApi = invitationApi;
        private readonly ILogger<InvitationService> _logger = logger;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public async Task<string?> GetEmailById(Guid invitationId)
        {
            var resposne = await _invitationApi.GetInvitationInfoAsync(invitationId);

            if (resposne.IsSuccess && resposne.Response is not null)
                return resposne.Response.Email;

            if (!string.IsNullOrWhiteSpace(resposne.Message))
            {
                _globalNotificationService.ShowError(resposne.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get invitation info failed: {Error}", resposne.Message);
            }

            return string.Empty;
        }

        public async Task<bool> SendInvitations(InviteUsersRequest request)
        {
            var resposne = await _invitationApi.SendInvitationsAsync(request);

            if (resposne.IsSuccess && resposne.Response is not null)
            {
                _globalNotificationService.ShowSuccess(resposne.Response);
                return true;
            }

            if (!string.IsNullOrWhiteSpace(resposne.Message))
            {
                _globalNotificationService.ShowError(resposne.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Send invitations failed: {Error}", resposne.Message);
            }

            return false;
        }
    }
}
