using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.GlobalNotification;
using Newtonsoft.Json.Linq;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetNotificationTypeInfo(NotificationType type) => type switch
        {
            NotificationType.Info => new EnumUIResult("Информация", string.Empty),
            NotificationType.Success => new EnumUIResult("Успех", "alert-success"),
            NotificationType.Error => new EnumUIResult("Ошибка", "alert-danger"),
            _ => new EnumUIResult(type.ToString(), string.Empty)
        };
    }
}
