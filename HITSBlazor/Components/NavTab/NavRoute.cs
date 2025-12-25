namespace HITSBlazor.Components.NavTab
{
    public class NavRoute
    {
        public string To { get; set; } = string.Empty;
        public string? Text { get; set; }
        public string? IconName { get; set; }
        public List<string>? Roles { get; set; }
    }
}
