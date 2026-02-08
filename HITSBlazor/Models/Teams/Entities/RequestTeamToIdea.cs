using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Teams.Enums;

namespace HITSBlazor.Models.Teams.Entities
{
    public class RequestTeamToIdea
    {
        public Guid IdeaMarketId { get; set; }
        public TeamRequestStatus Status { get; set; }
        public string Letter { get; set; } = string.Empty;
        public Guid TeamId { get; set; }
        public Guid MarketId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MembersCount { get; set; }
        public List<Skill> Skills { get; set; } = [];
    }
}
