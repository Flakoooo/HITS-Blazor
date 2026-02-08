using HITSBlazor.Models.Teams.Enums;

namespace HITSBlazor.Models.Teams.Entities
{
    public class TeamInvitation
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public Guid UserId { get; set; }
        public TeamRequestStatus Status { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
