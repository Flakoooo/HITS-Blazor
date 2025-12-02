using HITSBlazor.Models.Markets.Enums;

namespace HITSBlazor.Models.Markets.Entities
{
    public class Market
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string FinishDate { get; set; } = string.Empty;
        public MarketStatus Status { get; set; }
    }
}
