using HITSBlazor.Components.Placeholders.Placeholder;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetPlaceholderInfo(PlaceholderHeight height) => height switch
        {
            PlaceholderHeight.None => new EnumUIResult(),
            PlaceholderHeight.Smaller => new EnumUIResult("smaller", "height: 40px;"),
            PlaceholderHeight.Small => new EnumUIResult("small", "height: 50px;"),
            PlaceholderHeight.Regular => new EnumUIResult("regular", "height: 70px;"),
            PlaceholderHeight.Medium => new EnumUIResult("medium", "height: 100px;"),
            PlaceholderHeight.Second => new EnumUIResult("second", "height: 200px;"),
            _ => new EnumUIResult(height.ToString(), string.Empty)
        };
    }
}
