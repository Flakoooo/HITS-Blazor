using HITSBlazor.Models.Markets.Enums;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetMarketIdeaStatusInfo(IdeaMarketStatusType statusType) => statusType switch
        {
            IdeaMarketStatusType.RecruitmentIsOpen => new EnumUIResult("Набор открыт", "text-success"),
            IdeaMarketStatusType.RecruitmentIsClosed => new EnumUIResult("Набор закрыт", "text-danger"),
            IdeaMarketStatusType.Project => new EnumUIResult("Проект", "text-warning"),
            _ => new EnumUIResult(statusType.ToString(), string.Empty)
        };
    }
}
