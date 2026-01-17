namespace HITSBlazor.Utils.EnumStyles
{
    public static class IdeaRatingStyle
    {
        public static string GetStyle(this double rating) => rating switch
        {
            < 3.0 => "text-danger",
            >= 3.0 and < 4.0 => "text-warning",
            >= 4.0 => "text-success",
            _ => string.Empty
        };

        public static string GetStyle(this double? rating)
            => rating is null ? string.Empty : rating.Value.GetStyle();
    }
}
