using HITSBlazor.Utils.Attributes;
using System.ComponentModel;

namespace HITSBlazor.Models.Markets.Enums
{
    public enum IdeaMarketStatusType
    {
        [Description("Набор открыт")]
        [Style("text-success")]
        RecruitmentIsOpen,

        [Description("Набор закрыт")]
        [Style("text-danger")]
        RecruitmentIsClosed,

        [Description("Проект")]
        [Style("text-warning")]
        Project
    }
}
