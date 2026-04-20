using HITSBlazor.Components.Typography;

namespace HITSBlazor.Utils.EnumStyles
{
    public static class IdeaRatingStyle
    {
        public static TextColor GetStyle(this double rating) => rating switch
        {
            < 3.0 => TextColor.Danger,
            >= 3.0 and < 4.0 => TextColor.Warning,
            >= 4.0 => TextColor.Success,
            _ => TextColor.Secondary
        };

        public static TextColor GetStyle(this double? rating)
            => rating is null ? TextColor.Secondary : rating.Value.GetStyle();
    }
}
