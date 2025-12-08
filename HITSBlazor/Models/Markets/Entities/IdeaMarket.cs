using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Models.Markets.Entities
{
    public class IdeaMarket
    {
        public string Id { get; set; } = string.Empty;
        public string MarketId { get; set; } = string.Empty;
        public string IdeaId { get; set; } = string.Empty;
        public User Initiator { get; set; } = new();
        public string Name { get; set; } = string.Empty;
        public string Problem { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public int MaxTeamSize { get; set; }
        public string Customer { get; set; } = string.Empty;
        public int Position { get; set; }
        public Team? Team { get; set; }
        public List<Skill> Stack { get; set; } = [];
        public IdeaMarketStatusType Status { get; set; }
        public int Requests { get; set; }
        public int AcceptedRequests { get; set; }
        public bool IsFavorite { get; set; }
    }
}
