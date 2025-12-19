using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Models.Teams.Entities
{
    public class TeamMember
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<Skill> Skills { get; set; } = [];
    }
}
