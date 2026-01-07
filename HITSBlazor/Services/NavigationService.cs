using HITSBlazor.Components.LeftSideNavigation;
using HITSBlazor.Components.SelectActiveRoleModal;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Services
{
    public class NavigationService
    {
        private readonly NavigationManager _navigationManager;
        private readonly IAuthService _authService;
        private readonly ModalService _modalService;
        private readonly ILogger<NavigationService> _logger;

        public NavigationService(
            NavigationManager navigationManager,
            IAuthService authService,
            ModalService modalService,
            ILogger<NavigationService> logger
        )
        {
            _navigationManager = navigationManager;
            _authService = authService;
            _modalService = modalService;
            _logger = logger;

            _authService.OnActiveRoleChanged += OnActiveRoleChanged;
        }

        private const string DEFAULT_URL = "/ideas/list";

        public event Action? OnNavigationChanged;

        public async Task NavigateToAsync(string url, bool forceLoad = false)
        {
            try
            {
                _navigationManager.NavigateTo(url, forceLoad);

                OnNavigationChanged?.Invoke();
            }
            catch (Exception ex)
            {
                if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Error))
                    _logger.LogError(ex, "Ошибка при навигации на {Url}", url);
                throw;
            }
        }

        public async Task NavigateToDefaultAsync()
        {
            if (_authService.CurrentUser?.Role == null)
            {
                _modalService.Show<SelectActiveRoleModal>();
            }
            else
            {
                await NavigateToAsync(DEFAULT_URL);
            }
        }

        private async void OnActiveRoleChanged(RoleType? role)
        {
            await NavigateToAsync(DEFAULT_URL);
        }

        public static List<NavigationItem> GetMenuItems()
        {
            return
            [
                new NavigationItem("Реестр идей", "bi-lightbulb", 0,
                [
                    new NavigationSubItem("Список идей", "bi-list", 0, "/list"),
                    new NavigationSubItem("Создать идею", "bi-plus-lg", 1, "/create")
                ], "/ideas"),

                new NavigationItem("Реестр команд", "bi-people", 1,
                [
                    new NavigationSubItem("Список команд", "bi-list", 0, "/list"),
                    new NavigationSubItem("Создать команду", "bi-plus-lg", 1, "/create")
                ], "/teams"),
                new NavigationItem("Реестр бирж", "bi-shop-window", 2,
                [
                    new NavigationSubItem("Список бирж", "bi-list", 0, "/list"),
                    new NavigationSubItem("ACTIVE_MARKET_NAME", "bi-basket3", 1, "/guid")
                ], "/market"),
                new NavigationItem("Админ панель", "bi-ui-checks-grid", 3,
                [
                    new NavigationSubItem("Пользователи", "bi-person-gear", 0, "/users"),
                    new NavigationSubItem("Добавить пользователей", "bi-person-add", 1, "/add-users"),
                    new NavigationSubItem("Компании", "bi-building", 2, "/companies"),
                    new NavigationSubItem("Группы пользователей", "bi-people", 3, "/users-groups"),
                    new NavigationSubItem("Справочник компетенций", "bi-person-badge", 4, "/skills"),
                    new NavigationSubItem("Справочник тегов", "bi-tags", 5, "/tags")
                ], "/admin"),
                new NavigationItem("Реестр проектов", "bi-briefcase", 4,
                [
                    new NavigationSubItem("Список проектов", "bi-list", 0, "/list"),
                ], "/projects"),
                new NavigationItem("Тесты", "bi-clipboard", 5, [], "/tests/list")
            ];
        }

        public NavigationItem? GetCurrentMenuItem()
        {
            var currentUrl = _navigationManager.Uri;
            var menuItems = GetMenuItems();

            return menuItems.FirstOrDefault(item =>
                currentUrl.Contains(item.BaseUrl ?? "", StringComparison.OrdinalIgnoreCase));
        }

        public NavigationSubItem? GetCurrentSubItem()
        {
            var currentUrl = _navigationManager.Uri;
            var menuItems = GetMenuItems();

            foreach (var menuItem in menuItems)
            {
                var subItem = menuItem.SubItems.FirstOrDefault(sub =>
                    currentUrl.Equals(_navigationManager.ToAbsoluteUri($"{menuItem.BaseUrl}{sub.Url}").ToString(),
                        StringComparison.OrdinalIgnoreCase));

                if (subItem != null)
                    return subItem;
            }

            return null;
        }

        public void Dispose()
        {
            _authService.OnActiveRoleChanged -= OnActiveRoleChanged;
        }

    }
}
