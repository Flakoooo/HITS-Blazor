using HITSBlazor.Models.Users.Enums;
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

        private RoleType? CurrentRole { get; set; } = null;

        private bool isLoading = true;
        private bool isHovered = false;
        private (int ParentId, int SubItemId) activeSubItem = (0, 0);
        private List<NavigationItem> _menuItems = [];

        private static string NavigationLayout => NavigationStyles.Layout;
        private static string NavigationStyle => NavigationStyles.GetNavigationStyle();
        private static string NavigationContentStyle => NavigationStyles.GetNavigationContentStyle();

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            AuthService.OnActiveRoleChanged += RoleStateChanged;
            NavigationService.OnNavigationChanged += HandleNavigationChanged;
            _menuItems = NavigationService.MenuItems;
            UpdateActiveState();
            isLoading = false;
        }

        private async void RoleStateChanged(RoleType? role)
        {
            CurrentRole = role;
            _menuItems = NavigationService.MenuItems;
            UpdateActiveState();
            StateHasChanged();
        }

        private void HandleNavigationChanged()
        {
            UpdateActiveState();
        }

        private void UpdateActiveState()
        {
            var currentMenuItem = NavigationService.CurrentModule;
            var currentSubItem = NavigationService.CurrentPage;

            if (currentMenuItem != null)
            {
                activeSubItem = currentSubItem != null
                    ? (currentMenuItem.Id, currentSubItem.Id)
                    : (currentMenuItem.Id, -1);
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
            var baseClass = "list-group-item list-group-item-light d-flex w-100";
            var isActive = activeSubItem.ParentId == parentId && activeSubItem.SubItemId == subItemId;

            return isActive ? $"active router-link-exact-active {baseClass}" : baseClass;
        }

        private static string GetCollapseStyle(NavigationItem item)
        {
            if (!item.IsExpanded)
                return "height: 0px; overflow: hidden; transition: height 0.35s ease;";

            var height = 8 + (item.SubItems.Count * 40);
            return $"height: {height}px; overflow: hidden; transition: height 0.35s ease;";
        }

        private async Task SelectMenuItem(int itemId)
        {
            var item = _menuItems.FirstOrDefault(n => n.Id == itemId);
            if (item == null || string.IsNullOrEmpty(item.BaseUrl)) return;

            if (item.SubItems.Count != 0)
            {
                item.IsExpanded = !item.IsExpanded;

                if (item.SubItems.Count == 0)
                    activeSubItem = (itemId, -1);
            }
            else if (!string.IsNullOrEmpty(item.BaseUrl))
                await NavigationService.NavigateToAsync(item.BaseUrl);
        }

        private async Task SelectSubItem(int parentId, int subItemId)
        {
            var parentItem = _menuItems.FirstOrDefault(n => n.Id == parentId);
            var subItem = parentItem?.SubItems.FirstOrDefault(n => n.Id == subItemId);

            if (parentItem == null || subItem == null) return;

            await NavigationService.NavigateToAsync($"{parentItem.BaseUrl}{subItem.Url}");
        }

        public void Dispose()
        {
            AuthService.OnActiveRoleChanged -= RoleStateChanged;
            NavigationService.OnNavigationChanged -= HandleNavigationChanged;
        }
    }
}
