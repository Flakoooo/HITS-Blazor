using HITSBlazor.Utils.Attributes;
using System.ComponentModel;

namespace HITSBlazor.Components.GlobalNotification
{
    public enum NotificationType
    {
        [Description("Информация")]
        [Style("")]
        Info,

        [Description("Успех")]
        [Style("alert-success")]
        Success,

        [Description("Ошибка")]
        [Style("alert-danger")]
        Error
    }
}
