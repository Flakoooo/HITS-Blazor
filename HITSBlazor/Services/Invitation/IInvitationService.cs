using HITSBlazor.Models.Users.Requests;

namespace HITSBlazor.Services.Invitation
{
    public interface IInvitationService
    {
        Task<string?> GetEmailById(Guid invitationId);
        Task<bool> SendInvitations(InviteUsersRequest request);
    }
}
