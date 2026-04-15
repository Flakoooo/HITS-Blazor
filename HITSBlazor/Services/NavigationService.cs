using HITSBlazor.Components.LeftSideNavigation;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Markets;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace HITSBlazor.Services
{
    //TODO: думаю URL страниц как то унифицировать, рпвильное ли вообще слово?
    //TODO: сделать сохранение текущей страницы, если роль позволяет на ней находиться
    public class NavigationService
    {
        private readonly NavigationManager _navigationManager;
        private readonly IAuthService _authService;
        private readonly IMarketService _marketService;
        private readonly IProjectService _projectService;
        private readonly ModalService _modalService;
        private readonly ILogger<NavigationService> _logger;

        public NavigationService(
            NavigationManager navigationManager,
            IAuthService authService,
            IMarketService marketService,
            IProjectService projectService,
            ModalService modalService,
            ILogger<NavigationService> logger
        )
        {
            _navigationManager = navigationManager;
            _authService = authService;
            _marketService = marketService;
            _projectService = projectService;
            _modalService = modalService;
            _logger = logger;

            _authService.OnActiveRoleChanged += OnActiveRoleChanged;
            _navigationManager.LocationChanged += OnLocationChanged;
        }

        private const string DEFAULT_URL = "/ideas/list";

        public NavigationItem? CurrentModule { get; set; } = null;
        public NavigationSubItem? CurrentPage { get; set; } = null;
        public List<NavigationItem> MenuItems { get; set; } = [];

        public event Action? OnNavigationChanged;

        private async void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            try
            {
                await UpdateNavigationState(e.Location);
            }
            catch (Exception ex)
            {
                if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Error))
                    _logger.LogError(ex, "Ошибка при обработке изменения URL");
            }
        }

        private async Task UpdateNavigationState(string url)
        {
            var relativePath = GetRelativePath(url);

            if (string.IsNullOrEmpty(relativePath) || relativePath == "/" || relativePath == "redirect")
            {
                await NavigateToDefaultAsync();
                return;
            }

            CurrentModule = await GetCurrentMenuItem(relativePath);
            CurrentPage = GetCurrentSubItem(relativePath);

            OnNavigationChanged?.Invoke();
        }

        private string GetRelativePath(string fullUrl)
        {
            try
            {
                var pathAndQuery = new Uri(fullUrl).PathAndQuery;

                var baseUri = new Uri(_navigationManager.BaseUri);
                if (pathAndQuery.StartsWith(baseUri.AbsolutePath))
                    pathAndQuery = pathAndQuery[baseUri.AbsolutePath.Length..];

                return pathAndQuery;
            }
            catch
            {
                return fullUrl;
            }
        }

        public async Task NavigateToAsync(string url, bool forceLoad = false)
        {
            try
            {
                _navigationManager.NavigateTo(url, forceLoad);
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
                _modalService.ShowActiveRoleModal();
            else
                await NavigateToAsync(DEFAULT_URL);
        }

        private async void OnActiveRoleChanged(RoleType? role)
        {
            await Task.Delay(100);
            await GetSortedMenuItem();
            string url = DEFAULT_URL;
            var nav = MenuItems.FirstOrDefault();
            if (nav is not null)
            {
                var subNav = nav.SubItems.FirstOrDefault();
                if (subNav is not null)
                    url = $"{nav.BaseUrl}{subNav.Url}";
                else if (nav.SubItems.Count == 0)
                    url = nav.BaseUrl!;
            }
            await NavigateToAsync(url);
        }

        public async Task GetSortedMenuItem()
        {
            var menuItems = new List<NavigationItem>();
            var role = _authService.CurrentUser?.Role;
            if (role == null)
            {
                MenuItems = [];
                return;
            }

            var userRole = (RoleType) role;

            if (userRole is RoleType.Teacher or RoleType.Admin)
            {
                var nav = new NavigationItem
                {
                    Id = menuItems.Count,
                    Icon = "bi-ui-checks-grid",
                    Title = "Админ панель",
                    SubItems =
                    [
                        new NavigationSubItem
                        {
                            Id = 0,
                            Icon = "bi-person-gear",
                            Title = "Пользователи",
                            Url = "/users"
                        }
                    ],
                    BaseUrl = "admin"
                };

                if (userRole == RoleType.Admin)
                {
                    var subItems = new List<NavigationSubItem>()
                    {
                        new()
                        {
                            Id = 1,
                            Icon = "bi-person-add",
                            Title = "Добавить пользователей",
                            Url = "/add-users"
                        },
                        new()
                        {
                            Id = 2,
                            Icon = "bi-building-add",
                            Title = "Компании",
                            Url = "/companies"
                        },
                        new()
                        {
                            Id = 3,
                            Icon = "bi-people",
                            Title = "Группы пользователей",
                            Url = "/users-groups"
                        },
                        new()
                        {
                            Id = 4,
                            Icon = "bi-person-badge",
                            Title = "Справочник компетенций",
                            Url = "/skills"
                        },
                        new()
                        {
                            Id = 5,
                            Icon = "bi-tags",
                            Title = "Справочник тегов",
                            Url = "/tags"
                        }
                    };

                    nav.SubItems.AddRange(subItems);
                }

                menuItems.Add(nav);
            }

            var ideasRoles = new HashSet<RoleType>
            {
                RoleType.Initiator,
                RoleType.Member,
                RoleType.ProjectOffice,
                RoleType.Expert,
                RoleType.Admin,
                RoleType.Teacher
            };

            if (ideasRoles.Contains(userRole))
            {
                var nav = new NavigationItem
                {
                    Id = menuItems.Count,
                    Icon = "bi-lightbulb",
                    Title = "Реестр идей",
                    SubItems =
                    [
                        new NavigationSubItem
                        {
                            Id = 0,
                            Icon = "bi-list",
                            Title = "Список идей",
                            Url = "/list"
                        }
                    ],
                    BaseUrl = "ideas"
                };

                if (userRole is RoleType.Initiator or RoleType.Admin)
                {
                    var subNav = new NavigationSubItem
                    {
                        Id = nav.SubItems.Count,
                        Icon = "bi-plus-lg",
                        Title = "Создать идею",
                        Url = "/create"
                    };

                    nav.SubItems.Add(subNav);
                }

                menuItems.Add(nav);
            }

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

            var checkTeamsRoles = new HashSet<RoleType> 
            { 
                RoleType.ProjectOffice, 
                RoleType.Admin, 
                RoleType.Initiator 
            };

            bool teamsRolesAllowed = teamRoles.Contains(userRole);
            bool checkTeamsRolesAllowed = checkTeamsRoles.Contains(userRole);

            if (teamsRolesAllowed)
            {
                var nav = new NavigationItem
                {
                    Id = menuItems.Count,
                    Icon = "bi-people",
                    Title = "Реестр команд",
                    SubItems =
                    [
                        new NavigationSubItem
                        {
                            Id = 0,
                            Icon = "bi-list",
                            Title = "Список команд",
                            Url = "/list"
                        }
                    ],
                    BaseUrl = "teams"
                };

                if (userRole is RoleType.TeamOwner or RoleType.Admin)
                {
                    var subNav = new NavigationSubItem
                    {
                        Id = nav.SubItems.Count,
                        Icon = "bi-plus-lg",
                        Title = "Создать команду",
                        Url = "/create"
                    };

                    nav.SubItems.Add(subNav);
                }

                menuItems.Add(nav);
            }

            if (teamsRolesAllowed)
            {
                var nav = new NavigationItem
                {
                    Id = menuItems.Count,
                    Icon = "bi-shop-window",
                    Title = "Реестр бирж",
                    SubItems = [],
                    BaseUrl = "market"
                };

                if (checkTeamsRolesAllowed)
                {
                    var subItem1 = new NavigationSubItem
                    {
                        Id = nav.SubItems.Count,
                        Icon = "bi-list",
                        Title = "Список бирж",
                        Url = "/list"
                    };

                    nav.SubItems.Add(subItem1);
                }

                var activeMarket = await _marketService.GetMarketsAsync(selectedStatuses: [MarketStatus.Active]);
                int count = nav.SubItems.Count;
                foreach (var market in activeMarket)
                {
                    var subItem2 = new NavigationSubItem
                    {
                        Id = count,
                        Icon = "bi-basket3",
                        Title = market.Name,
                        Url = $"/{market.Id}"
                    };

                    nav.SubItems.Add(subItem2);
                    ++count;
                }

                menuItems.Add(nav);
            }

            if (teamsRolesAllowed)
            {
                var nav = new NavigationItem
                {
                    Id = menuItems.Count,
                    Icon = "bi-briefcase",
                    Title = "Реестр проектов",
                    SubItems = [],
                    BaseUrl = "projects"
                };

                if (checkTeamsRolesAllowed)
                {
                    var subItem1 = new NavigationSubItem
                    {
                        Id = nav.SubItems.Count,
                        Icon = "bi-list",
                        Title = "Список проектов",
                        Url = "/list"
                    };

                    nav.SubItems.Add(subItem1);
                }

                if (_authService.CurrentUser?.Id is not null)
                {
                    var projectAllowedRoles = new HashSet<RoleType>
                    {
                        RoleType.Initiator,
                        RoleType.Member,
                        RoleType.TeamOwner,
                        RoleType.TeamLeader
                    };
                    if (projectAllowedRoles.Contains(userRole))
                    {
                        var activeProjects = await _projectService.GetAllActiveProjects(_authService.CurrentUser.Id);
                        int count = nav.SubItems.Count;
                        foreach (var project in activeProjects)
                        {
                            var subItem2 = new NavigationSubItem
                            {
                                Id = count,
                                Icon = "bi-kanban",
                                Title = project.Name,
                                Url = $"/{project.Id}"
                            };

                            nav.SubItems.Add(subItem2);
                            ++count;
                        }
                    }
                }
                menuItems.Add(nav);
            }

            //var testRoles = new HashSet<RoleType>
            //{
            //    RoleType.Admin,
            //    RoleType.Member,
            //    RoleType.TeamLeader,
            //    RoleType.ProjectOffice
            //};

            //if (testRoles.Contains(userRole))
            //{
            //    var nav = new NavigationItem
            //    {
            //        Id = menuItems.Count,
            //        Icon = "bi-clipboard",
            //        Title = "Тесты",
            //        SubItems =
            //        [
            //            new NavigationSubItem
            //            {
            //                Id = 0,
            //                Icon = "bi-list",
            //                Title = "Список тестов",
            //                Url = "/list"
            //            },
            //            new NavigationSubItem
            //            {
            //                Id = 1,
            //                Icon = "bi-journal-text",
            //                Title = "Результаты тестов",
            //                Url = "/results"
            //            }
            //        ],
            //        BaseUrl = "tests"
            //    };

            //    menuItems.Add(nav);
            //}

            MenuItems = menuItems;
        }

        public async Task<NavigationItem?> GetCurrentMenuItem(string url)
        {
            var menuItems = MenuItems;

            foreach (var item in menuItems)
            {
                if (!string.IsNullOrEmpty(item.BaseUrl) &&
                    url.StartsWith(item.BaseUrl, StringComparison.OrdinalIgnoreCase))
                    return item;
            }

            return null;
        }

        public NavigationSubItem? GetCurrentSubItem(string url)
        {
            if (CurrentModule == null || CurrentModule.SubItems == null)
                return null;

            foreach (var item in CurrentModule.SubItems)
            {
                if (!string.IsNullOrEmpty(item.Url) &&
                    url.EndsWith(item.Url, StringComparison.OrdinalIgnoreCase))
                    return item;

                if (item.Url != null && item.Url.StartsWith('/') &&
                    url.Equals(item.Url, StringComparison.OrdinalIgnoreCase))
                    return item;
            }

            return null;
        }

        public void Dispose()
        {
            _authService.OnActiveRoleChanged -= OnActiveRoleChanged;
            _navigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}
