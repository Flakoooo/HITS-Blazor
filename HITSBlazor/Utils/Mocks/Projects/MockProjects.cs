using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Utils.Mocks.Ideas;
using HITSBlazor.Utils.Mocks.Markets;
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
                            TeamId = cardTeam.Id,
                            UserId = alex.Id,
                            Email = alex.Email,
                            FirstName = alex.FirstName,
                            LastName = alex.LastName,
                            StartDate = DateOnly.FromDateTime(new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc)),
                            FinishDate = DateOnly.FromDateTime(new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc)),
                            ProjectRole = ProjectMemberRole.Initiator
                        },
                        new ProjectMember
                        {
                            TeamId = cardTeam.Id,
                            UserId = kirill.Id,
                            Email = kirill.Email,
                            FirstName = kirill.FirstName,
                            LastName = kirill.LastName,
                            StartDate = DateOnly.FromDateTime(new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc)),
                            FinishDate = DateOnly.FromDateTime(new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc)),
                            ProjectRole = ProjectMemberRole.TeamLeader
                        },
                        new ProjectMember
                        {
                            TeamId = cardTeam.Id,
                            UserId = denis.Id,
                            Email = denis.Email,
                            FirstName = denis.FirstName,
                            LastName = denis.LastName,
                            StartDate = DateOnly.FromDateTime(new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc)),
                            FinishDate = DateOnly.FromDateTime(new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc)),
                            ProjectRole = ProjectMemberRole.Member
                        }
                    ],
                    StartDate = DateOnly.FromDateTime(new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc)),
                    FinishDate = DateOnly.FromDateTime(new DateTime(2024, 10, 25, 11, 2, 17, DateTimeKind.Utc)),
                    Status = ProjectStatus.Active
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
                            TeamId = cactusTeam.Id,
                            UserId = admin.Id,
                            Email = admin.Email,
                            FirstName = admin.FirstName,
                            LastName = admin.LastName,
                            StartDate = DateOnly.FromDateTime(new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc)),
                            FinishDate = DateOnly.FromDateTime(new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc)),
                            ProjectRole = ProjectMemberRole.Initiator
                        },
                        new ProjectMember
                        {
                            TeamId = cactusTeam.Id,
                            UserId = timur.Id,
                            Email = timur.Email,
                            FirstName = timur.FirstName,
                            LastName = timur.LastName,
                            StartDate = DateOnly.FromDateTime(new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc)),
                            FinishDate = DateOnly.FromDateTime(new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc)),
                            ProjectRole = ProjectMemberRole.TeamLeader
                        }
                    ],
                    Report = "репорт",
                    StartDate = DateOnly.FromDateTime(new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc)),
                    FinishDate = DateOnly.FromDateTime(new DateTime(2024, 1, 18, 11, 2, 17, DateTimeKind.Utc)),
                    Status = ProjectStatus.Done
                }
            ];
        }

        public static List<Project> GetAllMockProject() => _projects;

        public static ListDataResponse<Project> GetAllProjects(
            int page,
            int pageSize = 20,
            string? searchText = null,
            ProjectStatus? selectedStatus = null
        )
        {
            var query = _projects.AsEnumerable();

            if (selectedStatus.HasValue)
                query = query.Where(p => p.Status == selectedStatus);

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(p => p.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new ListDataResponse<Project>(count, query.ToList());
        }

        public static List<Project> GetActiveProjects(Guid id)
            => _projects.Where(p => p.Status == ProjectStatus.Active && p.Members.Select(m => m.UserId).Contains(id)).ToList();

        public static Project? GetProjectById(Guid projectId)
            => _projects.FirstOrDefault(p => p.Id == projectId);

        public static ListDataResponse<ProjectMember> GetProjectMembers(
            Guid projectId,
            int page,
            int pageSize = 20,
            string? searchText = null
        )
        {
            var project = _projects.FirstOrDefault(p => p.Id == projectId);
            if (project is null) return new ListDataResponse<ProjectMember>(0, []);

            var query = project.Members.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(m => 
                    m.FullName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) 
                    || m.Email.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)
                );

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new ListDataResponse<ProjectMember>(count, query.ToList());
        }

        public static ProjectMember? GetCurrentProjectMember(Guid projectId, Guid userId)
            => _projects.FirstOrDefault(p => p.Id == projectId)?.Members.FirstOrDefault(m => m.UserId == userId);

        public static bool CreateNewProject(IdeaMarket ideaMarket)
        {
            if (ideaMarket.Team is null) return false;

            var market = MockMarkets.GetMarketById(ideaMarket.MarketId);
            if (market is null) return false;

            var newProject = new Project
            {
                Id = Guid.NewGuid(),
                IdeaId = ideaMarket.IdeaId,
                Name = ideaMarket.Name,
                Description = ideaMarket.Description,
                Customer = ideaMarket.Customer,
                Initiator = ideaMarket.Initiator,
                Team = ideaMarket.Team,
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                FinishDate = market.FinishDate,
                Status = ProjectStatus.Active
            };

            newProject.Members.Add(new ProjectMember
            {
                UserId = ideaMarket.Initiator.Id,
                Email = ideaMarket.Initiator.Email,
                FirstName = ideaMarket.Initiator.FirstName,
                LastName = ideaMarket.Initiator.LastName,
                StartDate = newProject.StartDate,
                FinishDate = newProject.FinishDate,
                ProjectRole = ProjectMemberRole.Initiator
            });

            foreach (var member in ideaMarket.Team.Members)
            {
                newProject.Members.Add(new ProjectMember
                {
                    TeamId = ideaMarket.Team.Id,
                    UserId = member.Id,
                    Email = member.Email,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    StartDate = newProject.StartDate,
                    FinishDate = newProject.FinishDate,
                    ProjectRole = member.UserId == ideaMarket.Team.Leader?.Id ? ProjectMemberRole.TeamLeader : ProjectMemberRole.Member
                });
            }

            if (!MockIdeaMarkets.UpdateIdeaMarketStatus(ideaMarket.Id, IdeaMarketStatusType.Project))
                return false;

            _projects.Add(newProject);
            return true;
        }

        public static bool AddMemberInProject(Guid projectId, Guid memberId)
        {
            var user = MockUsers.GetUserById(memberId);
            if (user is null) return false;

            var project = _projects.FirstOrDefault(p => p.Id == projectId);
            if (project is null) return false;

            //TODOO: КАК НАЗНАЧАТЬ КОМАНДУ?
            project.Members.Add(new ProjectMember
            {
                TeamId = project.Team.Id,
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProjectRole = ProjectMemberRole.Member,
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                FinishDate = project.FinishDate
            });

            return true;
        }

        public static bool KickMemberFromProject(Guid projectId, Guid memberId)
        {
            var project = _projects.FirstOrDefault(p => p.Id == projectId);
            if (project is null) return false;

            var member = project.Members.FirstOrDefault(pm => pm.UserId == memberId);
            if (member is null) return false;
            if (project.Members.Remove(member))
            {
                MockSprintMarks.DeleteMarkByMemberId(projectId, memberId);
                MockAverageMarks.DeleteMarkByMemberId(memberId);
                return true;
            }

            return false;
        }

        public static Project? UpdateProjectStatus(Guid projectId, ProjectStatus newStatus)
        {
            if (newStatus is ProjectStatus.Done or ProjectStatus.Deleted) return null;

            var project = _projects.FirstOrDefault(p => p.Id == projectId);
            if (project is null) return null;

            project.Status = newStatus;

            return project;
        }

        public static Project? FinishProject(Guid projectId, string report)
        {
            var project = _projects.FirstOrDefault(p => p.Id == projectId);
            if (project is null) return null;

            project.Status = ProjectStatus.Done;
            project.Report = report;
            MockTeams.GetTeamById(project.Team.Id)?.HasActiveProject = false;

            return project;
        }

        public static Project? DeleteProject(Guid projectId)
        {
            var project = _projects.FirstOrDefault(p => p.Id == projectId);
            if (project is null) return null;

            project.Status = ProjectStatus.Deleted;
            MockTeams.GetTeamById(project.Team.Id)?.HasActiveProject = false;

            return project;
        }
    }
}
