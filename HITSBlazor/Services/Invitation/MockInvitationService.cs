using HITSBlazor.Models.Users.Requests;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Services.Invitation
{
    public class MockInvitationService(GlobalNotificationService globalNotificationService) : IInvitationService
    {
        private GlobalNotificationService _globalNotificationService = globalNotificationService;

        public async Task<string?> GetEmailById(Guid invitationId)
            => MockInvitation.GetEmailById(invitationId);

        public async Task<bool> SendInvitations(InviteUsersRequest request)
        {
            foreach (var email in request.Emails)
                MockInvitation.CreateInvitation(email, request.Roles);

            _globalNotificationService.ShowSuccess($"Новые приглашения успешно отправлены в кол-ве {request.Emails.Count}");
            return true;
        }
    }
}
