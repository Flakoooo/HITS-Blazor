using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.NavTab
{
    public partial class NavTab
    {
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? To { get; set; } = "#";
        [Parameter] public string? IconName { get; set; }
        [Parameter] public string? WrapperClassName { get; set; }
        [Parameter] public string? ClassName { get; set; }
        [Parameter] public bool IsActive { get; set; }
        [Parameter] public List<NavRoute>? Routes { get; set; }
        [Parameter] public Func<List<string>?, bool>? CheckUserRole { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        private bool isCollapsed = true;
        private string currentUrl = string.Empty;

        protected override void OnInitialized()
        {
            currentUrl = NavigationManager.Uri;
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            currentUrl = e.Location;
            StateHasChanged();
        }

        private string GetWrapperClass()
        {
            var classes = new List<string> { "nav-item" };

            if (!string.IsNullOrEmpty(WrapperClassName))
                classes.Add(WrapperClassName);

            return string.Join(" ", classes);
        }

        private string GetNavTabClass(bool isButton)
        {
            var classes = new List<string> { "nav-link d-flex w-100" };

            if (IsActive)
                classes.Add("active");

            if (!string.IsNullOrEmpty(ClassName))
                classes.Add(ClassName);

            if (isButton && CheckIsActiveRoute())
                classes.Add("bg-primary text-white");

            return string.Join(" ", classes);
        }

        private bool CheckIsActiveRoute()
        {
            if (string.IsNullOrEmpty(To) || To == "#")
                return false;

            return currentUrl.Contains(To, StringComparison.OrdinalIgnoreCase);
        }

        private string GetCollapseClass()
        {
            return isCollapsed ? "" : "show";
        }

        private void ToggleCollapse()
        {
            isCollapsed = !isCollapsed;
        }

        private List<NavRoute> GetFilteredRoutes()
        {
            if (Routes == null)
                return [];

            if (CheckUserRole == null)
                return Routes;

            return [.. Routes.Where(route =>
                route.Roles == null ||
                CheckUserRole(route.Roles)
            )];
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}
