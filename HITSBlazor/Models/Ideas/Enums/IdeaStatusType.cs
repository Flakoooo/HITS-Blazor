using System.ComponentModel;

namespace HITSBlazor.Models.Ideas.Enums
{
    public enum IdeaStatusType
    {
        [Description("Новая")]
        New,

        [Description("На редактировании")]
        OnEditing,

        [Description("На согласовании")]
        OnApproval,

        [Description("На утверждении")]
        OnConfirmation,

        [Description("Утверждена")]
        Confirmed,

        [Description("Опубликована")]
        OnMarket
    }
}
