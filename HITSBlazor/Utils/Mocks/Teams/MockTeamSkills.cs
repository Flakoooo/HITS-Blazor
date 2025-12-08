using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockTeamSkills
    {
        public static List<TeamSkills> GetMockTeamSkills() => [
            new TeamSkills
            {
                TeamId = MockTeams.CardId,
                WantedSkills = [
                    MockSkills.GetSkillById(MockSkills.JavaScriptId),
                    MockSkills.GetSkillById(MockSkills.RustId)
                ],
                Skills = []
            },
            new TeamSkills
            {
                TeamId = MockTeams.CactusId,
                WantedSkills = [
                    MockSkills.GetSkillById(MockSkills.JavaScriptId),
                    MockSkills.GetSkillById(MockSkills.PythonId),
                    MockSkills.GetSkillById(MockSkills.GoId),
                    MockSkills.GetSkillById(MockSkills.VueId),
                    MockSkills.GetSkillById(MockSkills.AngularId),
                    MockSkills.GetSkillById(MockSkills.NodeId),
                    MockSkills.GetSkillById(MockSkills.SQLite),
                    MockSkills.GetSkillById(MockSkills.MySQLId),
                    MockSkills.GetSkillById(MockSkills.PostgreSQLId),
                    MockSkills.GetSkillById(MockSkills.MongoDBId),
                    MockSkills.GetSkillById(MockSkills.DockerId),
                    MockSkills.GetSkillById(MockSkills.GitId),
                    MockSkills.GetSkillById(MockSkills.GrafanaId)
                ],
                Skills = [
                    MockSkills.GetSkillById(MockSkills.JavaScriptId),
                    MockSkills.GetSkillById(MockSkills.PythonId),
                    MockSkills.GetSkillById(MockSkills.CppId),
                    MockSkills.GetSkillById(MockSkills.VueId),
                    MockSkills.GetSkillById(MockSkills.AngularId),
                    MockSkills.GetSkillById(MockSkills.DockerId),
                    MockSkills.GetSkillById(MockSkills.GitId),
                    MockSkills.GetSkillById(MockSkills.GrafanaId),
                    MockSkills.GetSkillById(MockSkills.ElasticsearchId),
                    MockSkills.GetSkillById(MockSkills.PostgreSQLId),
                    MockSkills.GetSkillById(MockSkills.MySQLId),
                    MockSkills.GetSkillById(MockSkills.SQLite),
                    MockSkills.GetSkillById(MockSkills.RedisId)
                ]
            }
        ];

        public static TeamSkills? GetTeamSkillsByTeamId(string teamId)
            => GetMockTeamSkills().FirstOrDefault(ts => ts.TeamId == teamId);

        public static List<Skill> GetSkillsByTeamId(string teamId)
            => GetTeamSkillsByTeamId(teamId)?.Skills ?? [];

        public static List<Skill> GetWantedSkillsByTeamId(string teamId)
            => GetTeamSkillsByTeamId(teamId)?.WantedSkills ?? [];

        public static List<string> GetTeamIdsBySkill(string skillId)
            => [.. GetMockTeamSkills()
                  .Where(ts => ts.Skills.Any(s => s.Id == skillId))
                  .Select(ts => ts.TeamId)];

        public static List<string> GetTeamIdsByWantedSkill(string skillId)
            => [.. GetMockTeamSkills()
                  .Where(ts => ts.WantedSkills.Any(s => s.Id == skillId))
                  .Select(ts => ts.TeamId)];

        public static List<string> GetTeamIdsBySkillName(string skillName)
            => [.. GetMockTeamSkills()
                  .Where(ts => ts.Skills.Any(s => s.Name == skillName) ||
                               ts.WantedSkills.Any(s => s.Name == skillName))
                  .Select(ts => ts.TeamId)];

        public static bool TeamHasSkill(string teamId, string skillId)
            => GetSkillsByTeamId(teamId).Any(s => s.Id == skillId);

        public static bool TeamWantsSkill(string teamId, string skillId)
            => GetWantedSkillsByTeamId(teamId).Any(s => s.Id == skillId);
    }
}
