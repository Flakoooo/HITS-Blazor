using ApexCharts;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Markets;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Components;

using Task = System.Threading.Tasks.Task;

namespace HITSBlazor.Components.LeftSideNavigation
{
    public partial class LeftSideNavigation : IDisposable
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private IMarketService MarketService { get; set; } = null!;

        [Inject]
        private IProjectService ProjectService { get; set; } = null!;

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
            MarketService.OnMarketStatusHasUpdated += MarketStatusHasChanged;
            ProjectService.OnProjectStatusHasChanged += ProjectStatusHasChanged;

            _menuItems = NavigationService.MenuItems;
            UpdateActiveState();

            isLoading = false;
        }

        private async void RoleStateChanged(RoleType? role)
        {
            isLoading = true;
            StateHasChanged();

            CurrentRole = role;
            UpdateActiveState();

            isLoading = false;
            StateHasChanged();
        }

        private void HandleNavigationChanged()
        {
            isLoading = true;
            StateHasChanged();

            _menuItems = NavigationService.MenuItems;
            UpdateActiveState();

            isLoading = false;
            StateHasChanged();
        }

        //TODOO: продумать, как обновлять значения в боковой панели
        //делать запрос при добавлении?
        private void MarketStatusHasChanged(Market market)
        {
            isLoading = true;
            StateHasChanged();

            var teamRoles = new HashSet<RoleType>
            {
                RoleType.Initiator,
                RoleType.TeamOwner,
                RoleType.Member,
                RoleType.ProjectOffice,
                RoleType.Admin,
                RoleType.Teacher,
                RoleType.TeamLeader
            };

            if (market.Status is MarketStatus.Active)
            {
                var navItem = NavigationService.MenuItems.FirstOrDefault(n => n.BaseUrl?.Equals("market", StringComparison.CurrentCultureIgnoreCase) ?? false);
                navItem?.SubItems.Add(new NavigationSubItem
                {
                    Id = navItem.SubItems.Count,
                    Icon = "bi-basket3",
                    Title = market.Name,
                    Url = $"/{market.Id}"
                });
            }
            else
            {
                var navItem = NavigationService.MenuItems.FirstOrDefault(n => n.BaseUrl?.Equals("market", StringComparison.CurrentCultureIgnoreCase) ?? false);
                if (navItem is not null)
                {
                    var targetMarket = navItem.SubItems.FirstOrDefault(si => si.Url.Contains(market.Id.ToString(), StringComparison.CurrentCultureIgnoreCase));
                    if (targetMarket is not null && navItem.SubItems.Remove(targetMarket))
                    {
                        _menuItems = NavigationService.MenuItems;
                        UpdateActiveState();
                    }
                }
            }

            isLoading = false;
            StateHasChanged();
        }

        private void ProjectStatusHasChanged(Project project)
        {
            isLoading = true;
            StateHasChanged();

            if (project.Status is ProjectStatus.Active)
            {
                var navItem = NavigationService.MenuItems.FirstOrDefault(n => n.BaseUrl?.Equals("projects", StringComparison.CurrentCultureIgnoreCase) ?? false);
                navItem?.SubItems.Add(new NavigationSubItem
                {
                    Id = navItem.SubItems.Count,
                    Icon = "bi-kanban",
                    Title = project.Name,
                    Url = $"/{project.Id}"
                });
            }
            else
            {
                var navItem = NavigationService.MenuItems.FirstOrDefault(n => n.BaseUrl?.Equals("projects", StringComparison.CurrentCultureIgnoreCase) ?? false);
                if (navItem is not null)
                {
                    var targetMarket = navItem.SubItems.FirstOrDefault(si => si.Url.Contains(project.Id.ToString(), StringComparison.CurrentCultureIgnoreCase));
                    if (targetMarket is not null && navItem.SubItems.Remove(targetMarket))
                    {
                        _menuItems = NavigationService.MenuItems;
                        UpdateActiveState();
                    }
                }
            }

            isLoading = false;
            StateHasChanged();
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

            if (activeSubItem.ParentId == itemId)
                return $"{baseClass} bg-primary text-white";

            return isHovered ? $"{baseClass} text-white" : $"{baseClass} text-black";
        }

        private string GetSubItemClass(int parentId, int subItemId)
        {
            var baseClass = "list-group-item list-group-item-light d-flex w-100";
            var isActive = activeSubItem.ParentId == parentId && activeSubItem.SubItemId == subItemId;

            return isActive ? $"active router-link-exact-active {baseClass}" : baseClass;
        }

        private async Task SelectMenuItem(int itemId)
        {
            var item = _menuItems.FirstOrDefault(n => n.Id == itemId);
            if (item is null || string.IsNullOrEmpty(item.BaseUrl)) return;

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

            if (parentItem is null || subItem is null) return;

            await NavigationService.NavigateToAsync($"{parentItem.BaseUrl}{subItem.Url}");
        }

        private void ShowRoleModal() => ModalService.ShowActiveRoleModal();

        private async Task Logout() => await AuthService.LogoutAsync();

        public void Dispose()
        {
            AuthService.OnActiveRoleChanged -= RoleStateChanged;
            NavigationService.OnNavigationChanged -= HandleNavigationChanged;
            MarketService.OnMarketStatusHasUpdated -= MarketStatusHasChanged;
            ProjectService.OnProjectStatusHasChanged -= ProjectStatusHasChanged;
        }
    }
}
