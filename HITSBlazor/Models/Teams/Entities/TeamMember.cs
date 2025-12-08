using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Models.Teams.Entities
{
    public class TeamMember
    {
        public string Id { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<Skill> Skills { get; set; } = [];
    }
}
