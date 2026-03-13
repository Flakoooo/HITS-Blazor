using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Utils.Mocks.Common
{
    public static class MockInvitation
    {
        private static readonly List<Invitation> _invitations = [];

        public class Invitation
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public DateTime DateExpired { get; set; } = DateTime.UtcNow.AddDays(1);
            public string Email { get; set; } = string.Empty;
            public List<RoleType> Roles { get; set; } = [];
        }

        public static string? GetEmailById(Guid invitationId)
            => _invitations.FirstOrDefault(i => i.Id == invitationId)?.Email;

        public static void CreateInvitation(string email, List<RoleType> roles)
        {
            var newInvitation = new Invitation { Email = email, Roles = roles };
            _invitations.Add(newInvitation);

            Console.WriteLine(newInvitation.Id);
        }
    }
}
