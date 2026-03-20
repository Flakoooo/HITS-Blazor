using HITSBlazor.Models.Markets.Enums;

namespace HITSBlazor.Models.Markets.Entities
{
    public class Market
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public MarketStatus Status { get; set; }
    }
}
