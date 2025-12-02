using HITSBlazor.Models.Entities;

namespace HITSBlazor.Models.Markets.Entities
{
    public class IdeaMarketAdvertisement
    {
        public string Id { get; set; } = string.Empty;
        public string IdeaMarketId { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public User Sender { get; set; } = new();
        public List<string> CheckedBy { get; set; } = new();
    }
}
