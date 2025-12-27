using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Components.LeftSideBar
{
    public class LeftSideBarTab
    {
        public string Name { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public List<LeftSideBarTab>? Routes { get; set; }
        public string IconName { get; set; } = string.Empty;
        public List<RoleType> Roles { get; set; } = [];
    }
}
