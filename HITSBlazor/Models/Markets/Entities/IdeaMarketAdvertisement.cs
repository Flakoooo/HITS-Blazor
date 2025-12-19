using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Models.Markets.Entities
{
    public class IdeaMarketAdvertisement
    {
        public Guid Id { get; set; }
        public Guid IdeaMarketId { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public User Sender { get; set; } = new();
        public List<string> CheckedBy { get; set; } = [];
    }
}
