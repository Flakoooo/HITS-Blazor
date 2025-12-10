using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockTeams
    {
        private static readonly List<Team> _teams = CreateTeams();
        
        private static readonly string _lorem = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Eius aperiam delectus possimus, voluptates quo accusamus? Consequatur, quasi rem temporibus blanditiis delectus aliquid officia aut, totam incidunt reiciendis eaque laborum fugiat!";

        public static string CardId { get; } = Guid.NewGuid().ToString();
        public static string CactusId { get; } = Guid.NewGuid().ToString();
        public static string CarpId { get; } = Guid.NewGuid().ToString();

        private static List<Team> CreateTeams()
        {
            var teamTags = MockTeamTags.GetAllTeamTags();

            var kirill = MockTeamMembers.GetTeamMemberById(MockUsers.KirillId)!;
            var timur = MockTeamMembers.GetTeamMemberById(MockUsers.TimurId)!;

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

            return
            [
                new Team
                {
                    Id = CardId,
                    Name = "Визитка",
                    Closed = true,
                    CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Description = _lorem,
                    MembersCount = 2,
                    Owner = kirill,
                    Leader = kirill,
                    Members = [ kirill, MockTeamMembers.GetTeamMemberById(MockUsers.DenisId) ],
                    Skills = [.. skills],
                    Tags = teamTags[0],
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
                    HasActiveProject = false,
                    CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Description = _lorem,
                    MembersCount = 5,
                    Owner = timur,
                    Leader = timur,
                    Members = [ timur, MockTeamMembers.GetTeamMemberById(MockUsers.AdminId) ],
                    Skills = [..skills],
                    Tags = teamTags[1],
                    StatusQuest = false,
                    IsAcceptedToIdea = true,
                    WantedSkills = [..wantedSkills],
                    IsRefused = false
                }
            ];
        }

        public static Team? GetTeamById(string id) 
            => _teams.FirstOrDefault(t => t.Id == id);
    }
}
