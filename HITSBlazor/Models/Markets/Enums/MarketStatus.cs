using HITSBlazor.Utils.Attributes;
using System.ComponentModel;

namespace HITSBlazor.Models.Markets.Enums
{
    public enum MarketStatus
    {
        [Description("Новая")]
        [Style("bg-primary-subtle text-primary")]
        New,

        [Description("Активная")]
        [Style("bg-warning-subtle text-warning")]
        Active,

        [Description("Завершена")]
        [Style("bg-success-subtle text-success")]
        Done
    }
}
