using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Teams.Enums;

namespace HITSBlazor.Models.Teams.Entities
{
    public class InvitationTeamToIdea
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public Guid IdeaId { get; set; }
        public Guid InitiatorId { get; set; }
        public string IdeaName { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public TeamRequestStatus Status { get; set; }
        public int TeamMembersCount { get; set; }
        public List<Skill> Skills { get; set; } = [];
    }
}
