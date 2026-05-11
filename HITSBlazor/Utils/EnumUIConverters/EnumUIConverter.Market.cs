using HITSBlazor.Models.Markets.Enums;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetMarketsStatusInfo(MarketStatus status) => status switch
        {
            MarketStatus.New => new EnumUIResult("Новая", "bg-primary-subtle text-primary"),
            MarketStatus.Active => new EnumUIResult("Активная", "bg-warning-subtle text-warning"),
            MarketStatus.Done => new EnumUIResult("Завершена", "bg-success-subtle text-success"),
            _ => new EnumUIResult(status.ToString(), string.Empty)
        };
    }
}
