using HITSBlazor.Models.Common.Entities;
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
                    Skills = []
                });
            }

            return members;
        }

        private static List<Team> CreateTeams()
        {
            var javaScript = MockSkills.GetSkillById(MockSkills.JavaScriptId)!;

            var skills = new List<Skill>() 
            {
                javaScript,
                MockSkills.GetSkillById(MockSkills.KotlinId)!,
                MockSkills.GetSkillById(MockSkills.ReactId)!,
                MockSkills.GetSkillById(MockSkills.AngularId)!
            };

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
                    Skills = [.. skills],
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
                    Skills = [..skills],
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
                    Skills = [..skills],
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

        public static List<Team> GetAllTeams() => _teams;
    }
}
