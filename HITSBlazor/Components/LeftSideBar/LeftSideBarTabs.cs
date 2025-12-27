using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Components.LeftSideBar
{
    public static class LeftSideBarTabs
    {
        public static List<LeftSideBarTab> GetTabs() => [
            new()
            {
                Name = "ideas",
                Text = "Реестр идей",
                To = "/ideas",
                IconName = "bi bi-lightbulb",
                Roles =
                [
                    RoleType.INITIATOR,
                    RoleType.MEMBER,
                    RoleType.PROJECT_OFFICE,
                    RoleType.EXPERT,
                    RoleType.ADMIN,
                    RoleType.TEACHER
                ],
                Routes =
                [
                    new()
                    {
                        Name = "ideas-list",
                        Text = "Список идей",
                        To = "/ideas/list",
                        IconName = "bi bi-list",
                        Roles =
                        [
                            RoleType.INITIATOR,
                            RoleType.MEMBER,
                            RoleType.PROJECT_OFFICE,
                            RoleType.EXPERT,
                            RoleType.ADMIN,
                            RoleType.TEACHER
                        ]
                    },
                    new()
                    {
                        Name = "create-idea",
                        Text = "Создать идею",
                        To = "/ideas/create",
                        IconName = "bi bi-plus-lg",
                        Roles = [RoleType.INITIATOR, RoleType.ADMIN]
                    }
                ]
            },
            new()
            {
                Name = "teams",
                Text = "Реестр команд",
                To = "/teams",
                IconName = "bi bi-people",
                Roles =
                [
                    RoleType.INITIATOR,
                    RoleType.TEAM_OWNER,
                    RoleType.MEMBER,
                    RoleType.ADMIN,
                    RoleType.PROJECT_OFFICE,
                    RoleType.TEACHER,
                    RoleType.TEAM_LEADER
                ],
                Routes =
                [
                    new()
                    {
                        Name = "teams-list",
                        Text = "Список команд",
                        To = "/teams/list",
                        IconName = "bi bi-list",
                        Roles =
                        [
                            RoleType.INITIATOR,
                            RoleType.TEAM_OWNER,
                            RoleType.MEMBER,
                            RoleType.ADMIN,
                            RoleType.PROJECT_OFFICE,
                            RoleType.TEACHER,
                            RoleType.TEAM_LEADER
                        ]
                    },
                    new()
                    {
                        Name = "create-team",
                        Text = "Создать команду",
                        To = "/teams/create",
                        IconName = "bi bi-plus-lg",
                        Roles = [RoleType.TEAM_OWNER, RoleType.ADMIN]
                    }
                ]
            },
            new()
            {
                Name = "markets",
                Text = "Реестр бирж",
                To = "/market",
                IconName = "bi bi-shop-window",
                Roles =
                [
                    RoleType.INITIATOR,
                    RoleType.MEMBER,
                    RoleType.TEAM_OWNER,
                    RoleType.TEAM_LEADER,
                    RoleType.PROJECT_OFFICE,
                    RoleType.ADMIN,
                    RoleType.TEACHER
                ],
                Routes =
                [
                    new()
                    {
                        Name = "markets-list",
                        Text = "Список бирж",
                        To = "/market/list",
                        IconName = "bi bi-list",
                        Roles =
                        [
                            RoleType.PROJECT_OFFICE,
                            RoleType.ADMIN,
                            RoleType.INITIATOR
                        ]
                    }
                ]
            },
            new()
            {
                Name = "admin",
                Text = "Админ панель",
                To = "/admin",
                IconName = "bi bi-ui-checks-grid",
                Roles = [RoleType.ADMIN, RoleType.TEACHER],
                Routes =
                [
                    new()
                    {
                        Name = "admin-users",
                        To = "/admin/users",
                        Text = "Пользователи",
                        IconName = "bi bi-person-gear",
                        Roles = [RoleType.ADMIN, RoleType.TEACHER]
                    },
                    new()
                    {
                        Name = "admin-add-users",
                        To = "/admin/add-users",
                        Text = "Добавить пользователей",
                        IconName = "bi bi-person-add",
                        Roles = [RoleType.ADMIN]
                    },
                    new()
                    {
                        Name = "admin-companies",
                        To = "/admin/companies",
                        Text = "Компании",
                        IconName = "bi bi-building",
                        Roles = [RoleType.ADMIN]
                    },
                    new()
                    {
                        Name = "admin-users-groups",
                        To = "/admin/users-groups",
                        Text = "Группы пользователей",
                        IconName = "bi bi-people",
                        Roles = [RoleType.ADMIN]
                    },
                    new()
                    {
                        Name = "admin-skills",
                        To = "/admin/skills",
                        Text = "Справочник компетенций",
                        IconName = "bi bi-person-badge",
                        Roles = [RoleType.ADMIN]
                    },
                    new()
                    {
                        Name = "admin-tags",
                        To = "/admin/tags",
                        Text = "Справочник тегов",
                        IconName = "bi bi-tags",
                        Roles = [RoleType.ADMIN]
                    }
                ]
            },
            new()
            {
                Name = "projects",
                Text = "Реестр проектов",
                To = "/projects",
                IconName = "bi bi-briefcase",
                Roles =
                [
                    RoleType.INITIATOR,
                    RoleType.MEMBER,
                    RoleType.TEAM_OWNER,
                    RoleType.PROJECT_OFFICE,
                    RoleType.ADMIN,
                    RoleType.TEAM_LEADER,
                    RoleType.TEACHER
                ],
                Routes =
                [
                    new()
                    {
                        Name = "list",
                        Text = "Список проектов",
                        To = "/projects/list",
                        IconName = "bi bi-list",
                        Roles =
                        [
                            RoleType.PROJECT_OFFICE,
                            RoleType.ADMIN,
                            RoleType.TEACHER
                        ]
                    }
                ]
            },
            new()
            {
                Name = "questionnaire",
                Text = "Управление опросами",
                To = "/questionnaire",
                IconName = "bi bi-patch-question",
                Roles =
                [
                    RoleType.PROJECT_OFFICE,
                    RoleType.INITIATOR,
                    RoleType.TEAM_LEADER,
                    RoleType.MEMBER,
                    RoleType.TEACHER
                ]
            },
            new()
            {
                Name = "tests",
                Text = "Тесты",
                To = "/tests",
                IconName = "bi bi-clipboard",
                Roles =
                [
                    RoleType.ADMIN,
                    RoleType.MEMBER,
                    RoleType.TEAM_LEADER,
                    RoleType.PROJECT_OFFICE
                ]
            }
        ];
    }
}
