using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Markets.Enums;

namespace HITSBlazor.Models.Markets.Entities
{
    public class RequestTeamToIdea
    {
        public string IdeaMarketId { get; set; } = string.Empty;
        public RequestToIdeaStatus Status { get; set; }
        public string Letter { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public string MarketId { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int MembersCount { get; set; }
        public List<Skill> Skills { get; set; } = [];
    }
}
