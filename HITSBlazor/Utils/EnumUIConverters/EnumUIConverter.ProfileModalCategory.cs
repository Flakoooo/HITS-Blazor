using HITSBlazor.Components.Modals.RightSideModals.ProfileModal;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        public static EnumUIResult GetProfileCategoryInfo(ProfileModalCategory category) => category switch
        {
            ProfileModalCategory.General => new EnumUIResult("Общее", string.Empty),
            ProfileModalCategory.Skills => new EnumUIResult("Компетенции", string.Empty),
            ProfileModalCategory.Ideas => new EnumUIResult("Идеи", string.Empty),
            ProfileModalCategory.Teams => new EnumUIResult("Портфолио", string.Empty),
            ProfileModalCategory.Tests => new EnumUIResult("Тесты", string.Empty),
            _ => new EnumUIResult(category.ToString(), string.Empty)
        };
    }
}
