using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Markets.Enums;

namespace HITSBlazor.Models.Markets.Entities
{
    public class RequestTeamToIdea
    {
        public Guid IdeaMarketId { get; set; }
        public RequestToIdeaStatus Status { get; set; }
        public string Letter { get; set; } = string.Empty;
        public Guid TeamId { get; set; }
        public Guid MarketId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MembersCount { get; set; }
        public List<Skill> Skills { get; set; } = [];
    }
}
