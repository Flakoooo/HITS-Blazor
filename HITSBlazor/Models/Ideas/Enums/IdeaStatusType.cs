using HITSBlazor.Utils.Attributes;
using System.ComponentModel;

namespace HITSBlazor.Models.Ideas.Enums
{
    public enum IdeaStatusType
    {
        [Description("Новая")]
        [Style("bg-primary-subtle text-primary")]
        New,

        [Description("На редактировании")]
        [Style("bg-warning-subtle text-warning")]
        OnEditing,

        [Description("На согласовании")]
        [Style("bg-warning-subtle text-warning")]
        OnApproval,

        [Description("На утверждении")]
        [Style("bg-danger-subtle text-danger")]
        OnConfirmation,

        [Description("Утверждена")]
        [Style("bg-success-subtle text-success")]
        Confirmed,

        [Description("Опубликована")]
        [Style("bg-success-subtle text-success")]
        OnMarket
    }
}
