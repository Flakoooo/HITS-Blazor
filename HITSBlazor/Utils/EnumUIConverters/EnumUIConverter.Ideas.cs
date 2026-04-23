using HITSBlazor.Models.Ideas.Enums;
using Newtonsoft.Json.Linq;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetIdeasStatusInfo(IdeaStatusType status) => status switch
        {
            IdeaStatusType.New => new EnumUIResult("Новая", "bg-primary-subtle text-primary"),
            IdeaStatusType.OnEditing => new EnumUIResult("На редактировании", "bg-warning-subtle text-warning"),
            IdeaStatusType.OnApproval => new EnumUIResult("На согласовании", "bg-warning-subtle text-warning"),
            IdeaStatusType.OnConfirmation => new EnumUIResult("На утверждении", "bg-danger-subtle text-danger"),
            IdeaStatusType.Confirmed => new EnumUIResult("Утверждена", "bg-success-subtle text-success"),
            IdeaStatusType.OnMarket => new EnumUIResult("Опубликована", "bg-success-subtle text-success"),
            _ => new EnumUIResult(status.ToString(), string.Empty)
        };
    }
}
