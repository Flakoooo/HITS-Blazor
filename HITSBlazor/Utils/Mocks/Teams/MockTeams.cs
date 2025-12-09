using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockTeams
    {
        public static string CardId { get; } = Guid.NewGuid().ToString();
        public static string CactusId { get; } = Guid.NewGuid().ToString();
        public static string CarpId { get; } = Guid.NewGuid().ToString();

        public static List<Team> GetMockTeams()
        {
            var teamTags = MockTeamTags.GetMockTeamTags();

            return
            [
                new Team
                {
                    Id = CardId,
                    Name = "Визитка",
                    Closed = true,
                    CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Eius aperiam delectus possimus, voluptates quo accusamus? Consequatur, quasi rem temporibus blanditiis delectus aliquid officia aut, totam incidunt reiciendis eaque laborum fugiat!",
                    MembersCount = 2,
                    Owner = MockTeamMembers.GetTeamMemberById(MockUsers.KirillId)!,
                    Leader = MockTeamMembers.GetTeamMemberById(MockUsers.KirillId),
                    Members = [
                        MockTeamMembers.GetTeamMemberById(MockUsers.KirillId),
                        MockTeamMembers.GetTeamMemberById(MockUsers.DenisId)
                    ],
                    Skills = [
                        MockSkills.GetSkillById(MockSkills.JavaScriptId),
                        MockSkills.GetSkillById(MockSkills.SwiftId),
                        MockSkills.GetSkillById(MockSkills.GoId),
                        MockSkills.GetSkillById(MockSkills.VueId)
                    ],
                    Tags = teamTags[0],
                    WantedSkills = [
                        MockSkills.GetSkillById(MockSkills.JavaScriptId),
                        MockSkills.GetSkillById(MockSkills.NodeId),
                        MockSkills.GetSkillById(MockSkills.MongoDBId)
                    ],
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
                    Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Eius aperiam delectus possimus, voluptates quo accusamus? Consequatur, quasi rem temporibus blanditiis delectus aliquid officia aut, totam incidunt reiciendis eaque laborum fugiat!",
                    MembersCount = 5,
                    Owner = MockTeamMembers.GetTeamMemberById(MockUsers.TimurId)!,
                    Leader = MockTeamMembers.GetTeamMemberById(MockUsers.TimurId),
                    Members = [
                        MockTeamMembers.GetTeamMemberById(MockUsers.TimurId),
                        MockTeamMembers.GetTeamMemberById(MockUsers.AdminId)
                    ],
                    Skills = [
                        MockSkills.GetSkillById(MockSkills.JavaScriptId),
                        MockSkills.GetSkillById(MockSkills.SwiftId),
                        MockSkills.GetSkillById(MockSkills.GoId),
                        MockSkills.GetSkillById(MockSkills.VueId)
                    ],
                    Tags = teamTags[1],
                    StatusQuest = false,
                    IsAcceptedToIdea = true,
                    WantedSkills = [
                        MockSkills.GetSkillById(MockSkills.JavaScriptId),
                        MockSkills.GetSkillById(MockSkills.NodeId),
                        MockSkills.GetSkillById(MockSkills.MongoDBId)
                    ],
                    IsRefused = false
                },
                //new Team
                //{
                //    Id = CarpId,
                //    Name = "Карасики",
                //    Closed = false,
                //    CreatedAt = "2023-10-10T11:02:17Z",
                //    Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Eius aperiam delectus possimus, voluptates quo accusamus? Consequatur, quasi rem temporibus blanditiis delectus aliquid officia aut, totam incidunt reiciendis eaque laborum fugiat!",
                //    MembersCount = 6,
                //    Owner = teamMembers[0],
                //    Leader = teamMembers[0],
                //    Members = [teamMembers[0], teamMembers[2], teamMembers[3]],
                //    Skills = [skills[0], skills[4], skills[6], skills[9]],
                //    Tags = teamTags[2],
                //    WantedSkills = [skills[0], skills[11], skills[16]],
                //    IsRefused = false,
                //    HasActiveProject = false,
                //    IsAcceptedToIdea = true,
                //    StatusQuest = false
                //}
            ];
        }

        public static Team? GetTeamById(string id)
            => GetMockTeams().FirstOrDefault(t => t.Id == id);

        public static List<Team> GetOpenTeams()
            => [.. GetMockTeams().Where(t => !t.Closed)];

        public static List<Team> GetTeamsWithActiveProjects()
            => [.. GetMockTeams().Where(t => t.HasActiveProject)];

        public static List<Team> GetTeamsAcceptedToIdea()
            => [.. GetMockTeams().Where(t => t.IsAcceptedToIdea)];

        public static Team? GetTeamByName(string name)
            => GetMockTeams().FirstOrDefault(t => t.Name == name);

        public static List<Team> GetTeamsByOwner(string ownerId)
            => [.. GetMockTeams().Where(t => t.Owner.Id == ownerId)];

        public static List<Team> GetTeamsByMember(string userId)
            => [.. GetMockTeams().Where(t => t.Members.Any(m => m.UserId == userId))];
    }
}
