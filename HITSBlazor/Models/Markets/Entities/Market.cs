using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Utils.Models;

namespace HITSBlazor.Models.Markets.Entities
{
    public class Market : ViewModelBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public MarketStatus Status { get; set; }

        public override string GetDisplayInfo() => Name;
        public override object GetId() => Id;
    }
}
