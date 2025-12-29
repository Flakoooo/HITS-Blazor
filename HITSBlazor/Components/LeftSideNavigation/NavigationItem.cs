namespace HITSBlazor.Components.LeftSideNavigation
{
    public class NavigationItem(string title, string icon, int id, List<NavigationSubItem> subItems)
    {
        public string Title { get; } = title;
        public string Icon { get; } = icon;
        public int Id { get; } = id;
        public List<NavigationSubItem> SubItems { get; } = subItems;
        public bool IsExpanded { get; set; }
    }

    public class NavigationSubItem(
        string title, string icon, int id
    )
    {
        public string Title { get; } = title;
        public string Icon { get; } = icon;
        public int Id { get; } = id;
        public bool IsActive { get; set; }
    }
}
