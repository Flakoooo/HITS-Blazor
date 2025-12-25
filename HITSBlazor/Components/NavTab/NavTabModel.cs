namespace HITSBlazor.Components.NavTab
{
    public class NavTabModel
    {
        public string? Label { get; set; }
        public string? To { get; set; } = "#";
        public string? IconName { get; set; }
        public string? WrapperClassName { get; set; }
        public string? ClassName { get; set; }
        public bool IsActive { get; set; }
        public List<NavRoute>? Routes { get; set; }
        public Func<List<string>?, bool>? CheckUserRole { get; set; }
    }
}
