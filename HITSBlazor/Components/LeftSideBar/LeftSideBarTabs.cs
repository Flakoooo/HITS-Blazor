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
                    RoleType.Initiator,
                    RoleType.Member,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Teacher
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
                            RoleType.Initiator,
                            RoleType.Member,
                            RoleType.ProjectOffice,
                            RoleType.Expert,
                            RoleType.Admin,
                            RoleType.Teacher
                        ]
                    },
                    new()
                    {
                        Name = "create-idea",
                        Text = "Создать идею",
                        To = "/ideas/create",
                        IconName = "bi bi-plus-lg",
                        Roles = [RoleType.Initiator, RoleType.Admin]
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
                    RoleType.Initiator,
                    RoleType.TeamOwner,
                    RoleType.Member,
                    RoleType.Admin,
                    RoleType.ProjectOffice,
                    RoleType.Teacher,
                    RoleType.TeamLeader
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
                            RoleType.Initiator,
                            RoleType.TeamOwner,
                            RoleType.Member,
                            RoleType.Admin,
                            RoleType.ProjectOffice,
                            RoleType.Teacher,
                            RoleType.TeamLeader
                        ]
                    },
                    new()
                    {
                        Name = "create-team",
                        Text = "Создать команду",
                        To = "/teams/create",
                        IconName = "bi bi-plus-lg",
                        Roles = [RoleType.TeamOwner, RoleType.Admin]
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
                    RoleType.Initiator,
                    RoleType.Member,
                    RoleType.TeamOwner,
                    RoleType.TeamLeader,
                    RoleType.ProjectOffice,
                    RoleType.Admin,
                    RoleType.Teacher
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
                            RoleType.ProjectOffice,
                            RoleType.Admin,
                            RoleType.Initiator
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
                Roles = [RoleType.Admin, RoleType.Teacher],
                Routes =
                [
                    new()
                    {
                        Name = "admin-users",
                        To = "/admin/users",
                        Text = "Пользователи",
                        IconName = "bi bi-person-gear",
                        Roles = [RoleType.Admin, RoleType.Teacher]
                    },
                    new()
                    {
                        Name = "admin-add-users",
                        To = "/admin/add-users",
                        Text = "Добавить пользователей",
                        IconName = "bi bi-person-add",
                        Roles = [RoleType.Admin]
                    },
                    new()
                    {
                        Name = "admin-companies",
                        To = "/admin/companies",
                        Text = "Компании",
                        IconName = "bi bi-building",
                        Roles = [RoleType.Admin]
                    },
                    new()
                    {
                        Name = "admin-users-groups",
                        To = "/admin/users-groups",
                        Text = "Группы пользователей",
                        IconName = "bi bi-people",
                        Roles = [RoleType.Admin]
                    },
                    new()
                    {
                        Name = "admin-skills",
                        To = "/admin/skills",
                        Text = "Справочник компетенций",
                        IconName = "bi bi-person-badge",
                        Roles = [RoleType.Admin]
                    },
                    new()
                    {
                        Name = "admin-tags",
                        To = "/admin/tags",
                        Text = "Справочник тегов",
                        IconName = "bi bi-tags",
                        Roles = [RoleType.Admin]
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
                    RoleType.Initiator,
                    RoleType.Member,
                    RoleType.TeamOwner,
                    RoleType.ProjectOffice,
                    RoleType.Admin,
                    RoleType.TeamLeader,
                    RoleType.Teacher
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
                            RoleType.ProjectOffice,
                            RoleType.Admin,
                            RoleType.Teacher
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
                    RoleType.ProjectOffice,
                    RoleType.Initiator,
                    RoleType.TeamLeader,
                    RoleType.Member,
                    RoleType.Teacher
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
                    RoleType.Admin,
                    RoleType.Member,
                    RoleType.TeamLeader,
                    RoleType.ProjectOffice
                ]
            }
        ];
    }
}
