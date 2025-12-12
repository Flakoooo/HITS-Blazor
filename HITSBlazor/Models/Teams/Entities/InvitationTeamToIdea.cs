using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Enums;

namespace HITSBlazor.Models.Teams.Entities
{
    public class InvitationTeamToIdea
    {
        public string Id { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public string IdeaId { get; set; } = string.Empty;
        public string InitiatorId { get; set; } = string.Empty;
        public string IdeaName { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public InvitationTeamToIdeaStatus Status { get; set; }
        public int TeamMembersCount { get; set; }
        public List<Skill> Skills { get; set; } = [];
    }
}
