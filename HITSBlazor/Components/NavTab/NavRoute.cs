using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Components.NavTab
{
    public class NavRoute
    {
        public string Name { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string? IconName { get; set; }
        public List<RoleType>? Roles { get; set; }
    }
}
