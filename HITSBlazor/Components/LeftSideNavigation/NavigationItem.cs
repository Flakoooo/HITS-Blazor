namespace HITSBlazor.Components.LeftSideNavigation
{
    public class NavigationItem
    {
        public int Id { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<NavigationSubItem> SubItems { get; set; } = [];
        public string? BaseUrl { get; set; } = string.Empty;
        public bool IsExpanded { get; set; } = false;
    }

    public class NavigationSubItem
    {
        public int Id { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
