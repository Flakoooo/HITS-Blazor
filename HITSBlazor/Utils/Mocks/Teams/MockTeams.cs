using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockTeams
    {        
        private static readonly string _lorem = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Eius aperiam delectus possimus, voluptates quo accusamus? Consequatur, quasi rem temporibus blanditiis delectus aliquid officia aut, totam incidunt reiciendis eaque laborum fugiat!";

        public static Guid CardId { get; } = Guid.NewGuid();
        public static Guid CactusId { get; } = Guid.NewGuid();
        public static Guid CarpId { get; } = Guid.NewGuid();

        private static readonly List<Team> _teams = CreateTeams();

        private static List<TeamMember> CreateTeamMembers(Guid teamId, params User[] users)
        {
            var members = new List<TeamMember>();

            foreach (var user in users)
            {
                members.Add(new TeamMember
                {
                    Id = Guid.NewGuid(),
                    TeamId = teamId,
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Skills = MockUsersSkills.GetUserSkillsById(user.Id)
                });
            }

            return members;
        }

        private static List<Team> CreateTeams()
        {
            var javaScript = MockSkills.GetSkillById(MockSkills.JavaScriptId)!;

            var wantedSkills = new List<Skill>()
            {
                javaScript,
                MockSkills.GetSkillById(MockSkills.DockerId)!,
                MockSkills.GetSkillById(MockSkills.PostgreSQLId)!
            };

            var cardMembers = CreateTeamMembers(
                CardId, MockUsers.GetUserById(MockUsers.KirillId)!, MockUsers.GetUserById(MockUsers.DenisId)!
            );
            var kirill = cardMembers.FirstOrDefault(m => m.UserId == MockUsers.KirillId)!;

            var cactusMembers = CreateTeamMembers(
                CactusId, MockUsers.GetUserById(MockUsers.TimurId)!, MockUsers.GetUserById(MockUsers.AdminId)!
            );
            var timur = cactusMembers.FirstOrDefault(m => m.UserId == MockUsers.TimurId)!;

            var carpMembers = CreateTeamMembers(
                CarpId, MockUsers.GetUserById(MockUsers.LubovId)!, MockUsers.GetUserById(MockUsers.DmitryId)!, MockUsers.GetUserById(MockUsers.AntonId)!
            );
            var lubov = carpMembers.FirstOrDefault(m => m.UserId == MockUsers.LubovId)!;

            return
            [
                new Team
                {
                    Id = CardId,
                    Name = "Визитка",
                    Closed = false,
                    CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                    Description = _lorem,
                    MembersCount = 2,
                    Owner = kirill,
                    Leader = kirill,
                    Members = cardMembers,
                    Skills = [.. cardMembers.SelectMany(m => m.Skills).Distinct()],
                    Tags = new TeamTags { StudyGroups = ["ИИП-22-1", "ИСТНб-21"], StudyCourses = [Course.first, Course.second] },
                    WantedSkills = [.. wantedSkills],
                    IsRefused = false,
                    HasActiveProject = true,
                    IsAcceptedToIdea = true,
                    StatusQuest = false
                },
                new Team
                {
                    Id = CactusId,
                    Name = "Кактус",
                    Closed = false,
                    CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                    Description = _lorem,
                    MembersCount = 2,
                    Owner = timur,
                    Leader = timur,
                    Members = cactusMembers,
                    Skills = [.. cactusMembers.SelectMany(m => m.Skills).Distinct()],
                    Tags = new TeamTags { StudyGroups = ["ИИПб-23-1", "АСОИУ-22-1"], StudyCourses = [Course.second] },
                    WantedSkills = [..wantedSkills],
                    StatusQuest = false,
                    HasActiveProject = true,
                    IsAcceptedToIdea = true,
                    IsRefused = false
                },
                new Team
                {
                    Id = CarpId,
                    Name = "Карасики",
                    Closed = false,
                    CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                    Description = _lorem,
                    MembersCount = 3,
                    Owner = lubov,
                    Leader = lubov,
                    Members = carpMembers,
                    Skills = [.. carpMembers.SelectMany(m => m.Skills).Distinct()],
                    Tags = new TeamTags { StudyGroups = ["ИСТНб-21-2", "АСОИУ-20-1"], StudyCourses = [Course.third, Course.fourth] },
                    WantedSkills = [..wantedSkills],
                    IsRefused = false,
                    HasActiveProject = false,
                    IsAcceptedToIdea = false,
                    StatusQuest = false
                }
            ];
        }

        public static Team? GetTeamById(Guid id) 
            => _teams.FirstOrDefault(t => t.Id == id);

        public static ListDataResponse<Team> GetAllTeamsByQueryParams(
            int page,
            int pageSize = 20,
            string? searchText = null,
            bool? privacy = null,
            bool? hasActiveProject = null,
            HashSet<Guid>? searchSkillIds = null,
            string? orderBy = null,
            bool? byDescending = null
        )
        {
            var query = _teams.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(t => t.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            if (privacy.HasValue)
                query = query.Where(t => t.Closed == privacy);

            if (hasActiveProject.HasValue)
                query = query.Where(t => t.HasActiveProject == hasActiveProject);

            if (searchSkillIds?.Count > 0)
                query = query.Where(t => t.Skills.Any(s => searchSkillIds.Contains(s.Id)));

            if (!string.IsNullOrWhiteSpace(orderBy) && byDescending.HasValue)
            {
                query = (orderBy, byDescending.Value) switch
                {
                    (nameof(Team.Closed), true) => query.OrderByDescending(t => t.Closed),
                    (nameof(Team.Closed), false) => query.OrderBy(t => t.Closed),
                    (nameof(Team.Name), true) => query.OrderByDescending(t => t.Name),
                    (nameof(Team.Name), false) => query.OrderBy(t => t.Name),
                    (nameof(Team.HasActiveProject), true) => query.OrderByDescending(t => t.HasActiveProject),
                    (nameof(Team.HasActiveProject), false) => query.OrderBy(t => t.HasActiveProject),
                    (nameof(Team.MembersCount), true) => query.OrderByDescending(t => t.MembersCount),
                    (nameof(Team.MembersCount), false) => query.OrderBy(t => t.MembersCount),
                    (nameof(Team.CreatedAt), true) => query.OrderByDescending(t => t.CreatedAt),
                    (nameof(Team.CreatedAt), false) => query.OrderBy(t => t.CreatedAt),
                    _ => query
                };
            }

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new ListDataResponse<Team> { Count = count, List = query.ToList() };
        }

        public static List<Team> GetTeamsByOwnerIdOrLeaderId(Guid userId) 
            => [.. _teams.Where(t => t.Owner.UserId == userId || t.Leader?.UserId == userId)];

        public static bool DeleteTeam(Team team) => _teams.Remove(team);
    }
}
