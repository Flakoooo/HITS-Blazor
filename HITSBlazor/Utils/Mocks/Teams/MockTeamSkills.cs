using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockTeamSkills
    {
        private static readonly List<TeamSkills> _teamSkills = CreateTeamSkills();

        private static List<TeamSkills> CreateTeamSkills()
        {
            var javaScript = MockSkills.GetSkillById(MockSkills.JavaScriptId)!;

            var baseSkills = new List<Skill>() {
                javaScript,
                MockSkills.GetSkillById(MockSkills.PythonId)!,
                MockSkills.GetSkillById(MockSkills.VueId)!,
                MockSkills.GetSkillById(MockSkills.AngularId)!,
                MockSkills.GetSkillById(MockSkills.SQLite)!,
                MockSkills.GetSkillById(MockSkills.MySQLId)!,
                MockSkills.GetSkillById(MockSkills.PostgreSQLId)!,
                MockSkills.GetSkillById(MockSkills.DockerId)!,
                MockSkills.GetSkillById(MockSkills.GitId)!,
                MockSkills.GetSkillById(MockSkills.GrafanaId)!
            };

            return [
                new TeamSkills
                {
                    TeamId = MockTeams.CardId,
                    WantedSkills = [ javaScript, MockSkills.GetSkillById(MockSkills.RustId) ],
                    Skills = []
                },
                new TeamSkills
                {
                    TeamId = MockTeams.CactusId,
                    WantedSkills = [
                        .. baseSkills,
                        MockSkills.GetSkillById(MockSkills.GoId),
                        MockSkills.GetSkillById(MockSkills.NodeId),
                        MockSkills.GetSkillById(MockSkills.MongoDBId),
                    ],
                    Skills = [
                        .. baseSkills,
                        MockSkills.GetSkillById(MockSkills.CppId),
                        MockSkills.GetSkillById(MockSkills.ElasticsearchId),
                        MockSkills.GetSkillById(MockSkills.RedisId)
                    ]
                }
            ];
        }
    }
}
