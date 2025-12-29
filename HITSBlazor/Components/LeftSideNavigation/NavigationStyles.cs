namespace HITSBlazor.Components.LeftSideNavigation
{
    public static class NavigationStyles
    {
        public const string Layout = "height: 100vh; width: 85px; min-width: 85px; max-width: 85px;";

        public static string GetNavigationStyle() => string.Join("; ", new[]
        {
            "position: relative",
            "top: auto",
            "right: auto",
            "bottom: auto",
            "left: auto",
            "z-index: 8",
            "overflow-x: hidden",
            "display: flex",
            "flex-direction: column",
            "flex-wrap: nowrap",
            "align-items: stretch",
            "justify-content: space-between",
            "gap: 8px",
            "transition: all 0.15s ease-out;"
        });

        public static string GetNavigationContentStyle() => string.Join("; ", new[]
        {
            "width: 280px",
            "min-width: 280px",
            "max-width: 280px",
            "display: flex",
            "flex-direction: column",
            "flex-wrap: nowrap",
            "align-items: flex-start",
            "justify-content: flex-start",
            "gap: 8px;"
        });
    }
}
