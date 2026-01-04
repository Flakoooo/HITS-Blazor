using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.LeftSideNavigation
{
    public partial class LeftSideNavigation : IDisposable
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        private User? CurrentUser => AuthService.CurrentUser;

        private bool isLoading = false;
        private bool isHovered = false;
        private (int ParentId, int SubItemId) activeSubItem = (0, 0);
        private List<NavigationItem> _menuItems = [];

        private static string NavigationLayout => NavigationStyles.Layout;
        private static string NavigationStyle => NavigationStyles.GetNavigationStyle();
        private static string NavigationContentStyle => NavigationStyles.GetNavigationContentStyle();

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            NavigationService.OnNavigationChanged += HandleNavigationChanged;
            _menuItems = NavigationService.GetMenuItems();
            UpdateActiveState();
            isLoading = false;
        }

        private void HandleNavigationChanged() => UpdateActiveState();

        private void UpdateActiveState()
        {
            var currentSubItem = NavigationService.GetCurrentSubItem();
            var currentMenuItem = NavigationService.GetCurrentMenuItem();

            if (currentMenuItem != null)
            {
                activeSubItem = currentSubItem != null
                    ? (currentMenuItem.Id, currentSubItem.Id)
                    : (currentMenuItem.Id, -1);

                foreach (var menuItem in _menuItems)
                    menuItem.IsExpanded = menuItem.Id == currentMenuItem.Id &&
                                              (currentSubItem != null || menuItem.SubItems.Count != 0);
            }
        }

        private string GetSidebarStyles() => isHovered
            ? "width: 312px; background-color: rgba(0, 0, 0, 0.729); color: #fff;"
            : "width: 80px; background-color: transparent; color: inherit;";

        private string GetItemButtonClass(int itemId)
        {
            var baseClass = "nav-link d-flex w-100";
            var isActive = activeSubItem.ParentId == itemId;

            if (isActive)
                return $"{baseClass} bg-primary text-white";

            return isHovered ? $"{baseClass} text-white" : $"{baseClass} text-black";
        }

        private string GetSubItemClass(int parentId, int subItemId)
        {
            var baseClass = "nav-route list-group-item list-group-item-light";
            var isActive = activeSubItem.ParentId == parentId && activeSubItem.SubItemId == subItemId;

            return isActive ? $"{baseClass} active router-link-exact-active" : baseClass;
        }

        private static string GetCollapseStyle(NavigationItem item)
        {
            if (!item.IsExpanded)
                return "height: 0px; overflow: hidden; transition: height 0.35s ease;";

            var height = 8 + (item.SubItems.Count * 40);
            return $"height: {height}px; overflow: hidden; transition: height 0.35s ease;";
        }

        private void HandleMouseEnter() => isHovered = true;
        private void HandleMouseLeave() => isHovered = false;

        private async Task SelectSubItem(int parentId, int subItemId)
        {
            var parentItem = _menuItems.FirstOrDefault(n => n.Id == parentId);
            var subItem = parentItem?.SubItems.FirstOrDefault(n => n.Id == subItemId);

            if (parentItem == null || subItem == null) return;

            await NavigationService.NavigateToAsync($"{parentItem.BaseUrl}{subItem.Url}");
        }

        private async Task SelectMenuItem(int itemId)
        {
            var item = _menuItems.FirstOrDefault(n => n.Id == itemId);
            if (item == null || string.IsNullOrEmpty(item.BaseUrl)) return;

            if (item.SubItems.Count != 0)
            {
                foreach (var menuItem in _menuItems)
                    if (menuItem.Id != itemId && menuItem.IsExpanded)
                        menuItem.IsExpanded = false;

                item.IsExpanded = !item.IsExpanded;

                if (item.SubItems.Count == 0)
                    activeSubItem = (itemId, -1);
            }
            else if (!string.IsNullOrEmpty(item.BaseUrl))
                await NavigationService.NavigateToAsync(item.BaseUrl);
        }

        public void Dispose()
        {
            NavigationService.OnNavigationChanged -= HandleNavigationChanged;
        }
    }
}
