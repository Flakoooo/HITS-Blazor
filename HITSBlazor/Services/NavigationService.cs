using HITSBlazor.Components.LeftSideNavigation;
using HITSBlazor.Components.Modals.SelectActiveRoleModal;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Markets;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace HITSBlazor.Services
{
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
            {
                _modalService.Show<SelectActiveRoleModal>(blockCloseModal: true);
            }
            else
            {
                await NavigateToAsync(DEFAULT_URL);
            }
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

            var ideasRoles = new List<RoleType>
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
                            AllowedRoles = ideasRoles,
                            Url = "/list"
                        }
                    ],
                    AllowedRoles = ideasRoles,
                    BaseUrl = "ideas"
                };

                var createIdeaRoles = new List<RoleType> { RoleType.Initiator, RoleType.Admin };
                if (createIdeaRoles.Contains(userRole))
                {
                    var subNav = new NavigationSubItem
                    {
                        Id = nav.SubItems.Count,
                        Icon = "bi-plus-lg",
                        Title = "Создать идею",
                        AllowedRoles = createIdeaRoles,
                        Url = "/create"
                    };

                    nav.SubItems.Add(subNav);
                }

                menuItems.Add(nav);
            }

            var teamRoles = new List<RoleType>
            {
                RoleType.Initiator,
                RoleType.TeamOwner,
                RoleType.Member,
                RoleType.ProjectOffice,
                RoleType.Admin,
                RoleType.Teacher,
                RoleType.TeamLeader
            };

            var checkTeamsRoles = new List<RoleType> 
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
                            AllowedRoles = teamRoles,
                            Url = "/list"
                        }
                    ],
                    AllowedRoles = teamRoles,
                    BaseUrl = "teams"
                };

                var createTeamRoles = new List<RoleType> { RoleType.TeamOwner, RoleType.Admin };
                if (createTeamRoles.Contains(userRole))
                {
                    var subNav = new NavigationSubItem
                    {
                        Id = nav.SubItems.Count,
                        Icon = "bi-plus-lg",
                        Title = "Создать команду",
                        AllowedRoles = createTeamRoles,
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
                    AllowedRoles = teamRoles,
                    BaseUrl = "market"
                };

                if (checkTeamsRolesAllowed)
                {
                    var subItem1 = new NavigationSubItem
                    {
                        Id = nav.SubItems.Count,
                        Icon = "bi-list",
                        Title = "Список бирж",
                        AllowedRoles = checkTeamsRoles,
                        Url = "/list"
                    };

                    nav.SubItems.Add(subItem1);
                }

                var activeMarket = await _marketService.GetActiveMarketsAsync();
                int count = nav.SubItems.Count;
                foreach (var market in activeMarket)
                {
                    var subItem2 = new NavigationSubItem
                    {
                        Id = count,
                        Icon = "bi-basket3",
                        Title = market.Name,
                        AllowedRoles = teamRoles,
                        Url = $"/{market.Id}"
                    };

                    nav.SubItems.Add(subItem2);
                    ++count;
                }

                menuItems.Add(nav);
            }

            var adminRoles = new List<RoleType> { RoleType.Admin, RoleType.Teacher };

            if (adminRoles.Contains(userRole))
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
                            AllowedRoles = adminRoles,
                            Url = "/users"
                        }
                    ],
                    AllowedRoles = adminRoles,
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
                            AllowedRoles = [RoleType.Admin],
                            Url = "/add-users"
                        },
                        new()
                        {
                            Id = 2,
                            Icon = "bi-building",
                            Title = "Компании",
                            AllowedRoles = [RoleType.Admin],
                            Url = "/companies"
                        },
                        new()
                        {
                            Id = 3,
                            Icon = "bi-people",
                            Title = "Группы пользователей",
                            AllowedRoles = [RoleType.Admin],
                            Url = "/users-groups"
                        },
                        new()
                        {
                            Id = 4,
                            Icon = "bi-person-badge",
                            Title = "Справочник компетенций",
                            AllowedRoles = [RoleType.Admin],
                            Url = "/skills"
                        },
                        new()
                        {
                            Id = 5,
                            Icon = "bi-tags",
                            Title = "Справочник тегов",
                            AllowedRoles = [RoleType.Admin],
                            Url = "/tags"
                        }
                    };

                    nav.SubItems.AddRange(subItems);
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
                    AllowedRoles = teamRoles,
                    BaseUrl = "projects"
                };

                if (checkTeamsRolesAllowed)
                {
                    var subItem1 = new NavigationSubItem
                    {
                        Id = nav.SubItems.Count,
                        Icon = "bi-list",
                        Title = "Список проектов",
                        AllowedRoles = checkTeamsRoles,
                        Url = "/list"
                    };

                    nav.SubItems.Add(subItem1);
                }

                if (_authService.CurrentUser?.Id is not null)
                {
                    var activeProjects = await _projectService.GetAllActiveProjects(_authService.CurrentUser.Id);
                    var projectAllowedRoles = new List<RoleType> 
                    { 
                        RoleType.Initiator,
                        RoleType.Member,
                        RoleType.TeamOwner,
                        RoleType.TeamLeader
                    };
                    int count = nav.SubItems.Count;
                    foreach (var project in activeProjects)
                    {
                        var subItem2 = new NavigationSubItem
                        {
                            Id = count,
                            Icon = "bi-kanban",
                            Title = project.Name,
                            AllowedRoles = projectAllowedRoles,
                            Url = $"/{project.Id}"
                        };

                        nav.SubItems.Add(subItem2);
                        ++count;
                    }
                }

            }

            var questRoles = new List<RoleType>
            {
                RoleType.ProjectOffice,
                RoleType.Initiator,
                RoleType.TeamLeader,
                RoleType.Member,
                RoleType.Teacher,
                RoleType.Admin
            };

            if (questRoles.Contains(userRole))
            {
                var nav = new NavigationItem
                {
                    Id = menuItems.Count,
                    Icon = "bi-patch-question",
                    Title = "Управление опросами",
                    SubItems = [],
                    AllowedRoles = questRoles,
                    BaseUrl = "questionnaire"
                };

                menuItems.Add(nav);
            }

            var testRoles = new List<RoleType>
            {
                RoleType.Admin,
                RoleType.Member,
                RoleType.TeamLeader,
                RoleType.ProjectOffice
            };

            if (testRoles.Contains(userRole))
            {
                var nav = new NavigationItem
                {
                    Id = menuItems.Count,
                    Icon = "bi-clipboard",
                    Title = "Тесты",
                    SubItems = [],
                    AllowedRoles = testRoles,
                    BaseUrl = "tests"
                };

                menuItems.Add(nav);
            }

            MenuItems = menuItems;
        }

        public async Task<NavigationItem?> GetCurrentMenuItem(string url)
        {
            var menuItems = MenuItems;

            foreach (var item in menuItems)
            {
                if (!string.IsNullOrEmpty(item.BaseUrl) &&
                    url.StartsWith(item.BaseUrl, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
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
                {
                    return item;
                }

                if (item.Url != null && item.Url.StartsWith('/') &&
                    url.Contains(item.Url, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
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
