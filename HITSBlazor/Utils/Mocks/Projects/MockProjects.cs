using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Utils.Mocks.Ideas;
using HITSBlazor.Utils.Mocks.Teams;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Projects
{
    public static class MockProjects
    {
        public static Guid ChatBotId { get; } = Guid.NewGuid();
        public static Guid ArmatureId { get; } = Guid.NewGuid();

        private static readonly List<Project> _projects = CreateProjects();

        private static List<Project> CreateProjects()
        {
            var chatBotIdea = MockIdeas.GetIdeaById(MockIdeas.ChatBotId)!;
            var armatureIdea = MockIdeas.GetIdeaById(MockIdeas.ArmatureId)!;

            var cardTeam = MockTeams.GetTeamById(MockTeams.CardId)!;
            var cactusTeam = MockTeams.GetTeamById(MockTeams.CactusId)!;

            var kirill = MockUsers.GetUserById(MockUsers.KirillId)!;
            var alex = MockUsers.GetUserById(MockUsers.AlexId)!;
            var denis = MockUsers.GetUserById(MockUsers.DenisId)!;
            var timur = MockUsers.GetUserById(MockUsers.TimurId)!;
            var admin = MockUsers.GetUserById(MockUsers.AdminId)!;

            var chatBotProjectName = "Чат-бот в telegram для запросов и обращений к HR вне системы 1С";
            var armatureProjectName = "Прогнозирование закупок арматуры на основе исторических данных и обогащением доп. критериями";

            return
            [
                new Project
                {
                    Id = ChatBotId,
                    IdeaId = chatBotIdea.Id,
                    Name = chatBotProjectName,
                    Description = chatBotIdea.Description,
                    Customer = chatBotIdea.Customer,
                    Initiator = chatBotIdea.Initiator,
                    Team = cardTeam,
                    Members =
                    [
                        new ProjectMember
                        {
                            ProjectName = chatBotProjectName,
                            TeamId = cardTeam.Id,
                            TeamName = cardTeam.Name,
                            UserId = alex.Id,
                            Email = alex.Email,
                            FirstName = alex.FirstName,
                            LastName = alex.LastName,
                            StartDate = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                            FinishDate = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                            ProjectRole = ProjectMemberRole.INITIATOR
                        },
                        new ProjectMember
                        {
                            ProjectName = chatBotProjectName,
                            TeamId = cardTeam.Id,
                            TeamName = cardTeam.Name,
                            UserId = kirill.Id,
                            Email = kirill.Email,
                            FirstName = kirill.FirstName,
                            LastName = kirill.LastName,
                            StartDate = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                            FinishDate = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                            ProjectRole = ProjectMemberRole.TEAM_LEADER
                        },
                        new ProjectMember
                        {
                            ProjectName = chatBotProjectName,
                            TeamId = cardTeam.Id,
                            TeamName = cardTeam.Name,
                            UserId = denis.Id,
                            Email = denis.Email,
                            FirstName = denis.FirstName,
                            LastName = denis.LastName,
                            StartDate = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                            FinishDate = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                            ProjectRole = ProjectMemberRole.MEMBER
                        }
                    ],
                    Report = new ReportProject
                    {
                        ProjectId = ChatBotId,
                        Marks = MockAverageMarks.GetAverageMarkByProjectId(ChatBotId),
                        Report = "Это отчет"
                    },
                    StartDate = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    FinishDate = "",
                    Status = ProjectStatus.ACTIVE
                },
                new Project
                {
                    Id = ArmatureId,
                    IdeaId = armatureIdea.Id,
                    Name = armatureProjectName,
                    Description = armatureIdea.Description,
                    Customer = armatureIdea.Customer,
                    Initiator = armatureIdea.Initiator,
                    Team = cactusTeam,
                    Members =
                    [
                        new ProjectMember
                        {
                            ProjectName = armatureProjectName,
                            TeamId = cactusTeam.Id,
                            TeamName = cactusTeam.Name,
                            UserId = admin.Id,
                            Email = admin.Email,
                            FirstName = admin.FirstName,
                            LastName = admin.LastName,
                            StartDate = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                            FinishDate = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                            ProjectRole = ProjectMemberRole.INITIATOR
                        },
                        new ProjectMember
                        {
                            ProjectName = armatureProjectName,
                            TeamId = cactusTeam.Id,
                            TeamName = cactusTeam.Name,
                            UserId = timur.Id,
                            Email = timur.Email,
                            FirstName = timur.FirstName,
                            LastName = timur.LastName,
                            StartDate = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                            FinishDate = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                            ProjectRole = ProjectMemberRole.TEAM_LEADER
                        }
                    ],
                    Report = new ReportProject
                    {
                        ProjectId = ArmatureId,
                        Marks = [],
                        Report = ""
                    },
                    StartDate = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    FinishDate = new DateTime(2024, 1, 18, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Status = ProjectStatus.DONE
                }
            ];
        }

        public static List<Project> GetActiveProjects(Guid id)
            => [.. _projects.Where(p => p.Id == id && p.Status == ProjectStatus.ACTIVE)];
    }
}
