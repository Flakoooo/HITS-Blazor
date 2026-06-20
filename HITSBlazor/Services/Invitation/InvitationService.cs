using HITSBlazor.Models.Users.Requests;

namespace HITSBlazor.Services.Invitation
{
    public class InvitationService(GlobalNotificationService globalNotificationService) : IInvitationService
    {
        private GlobalNotificationService _globalNotificationService = globalNotificationService;

        public async Task<string?> GetEmailById(Guid invitationId)
        {
            _globalNotificationService.ShowError("Метод GetEmailById не реализован");
            return string.Empty;
        }

        public async Task<bool> SendInvitations(InviteUsersRequest request)
        {
            _globalNotificationService.ShowError("Метод SendInvitations не реализован");
            return false;
        }
    }
}
