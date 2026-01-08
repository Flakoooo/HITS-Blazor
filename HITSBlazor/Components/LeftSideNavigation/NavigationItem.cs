namespace HITSBlazor.Components.LeftSideNavigation
{
    public class NavigationItem(string title, string icon, int id, List<NavigationSubItem> subItems, string? baseUrl = null)
    {
        public int Id { get; } = id;
        public string Icon { get; } = icon;
        public string Title { get; } = title;
        public List<NavigationSubItem> SubItems { get; } = subItems;
        public string? BaseUrl { get; } = baseUrl;
        public bool IsExpanded { get; set; }
    }

    public class NavigationSubItem(
        string title, string icon, int id, string url
    )
    {
        public int Id { get; } = id;
        public string Icon { get; } = icon;
        public string Title { get; } = title;
        public string Url { get; } = url;
        public bool IsActive { get; set; }
    }
}
