using HITSBlazor.Models.Markets.Enums;

namespace HITSBlazor.Models.Markets.Entities
{
    public class Market
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string FinishDate { get; set; } = string.Empty;
        public MarketStatus Status { get; set; }
    }
}
