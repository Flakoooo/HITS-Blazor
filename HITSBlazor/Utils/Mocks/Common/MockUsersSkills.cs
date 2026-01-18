using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Common
{
    public static class MockUsersSkills
    {
        private static readonly List<UsersSkills> _usersSkills = CreateUsersSkills();

        private static List<UsersSkills> CreateUsersSkills()
        {
            var javaScript = MockSkills.GetSkillById(MockSkills.JavaScriptId)!;
            var kotlin = MockSkills.GetSkillById(MockSkills.KotlinId)!;
            var react = MockSkills.GetSkillById(MockSkills.ReactId)!;
            var vue = MockSkills.GetSkillById(MockSkills.VueId)!;
            var python = MockSkills.GetSkillById(MockSkills.PythonId)!;
            var rust = MockSkills.GetSkillById(MockSkills.RustId)!;
            var cpp = MockSkills.GetSkillById(MockSkills.CppId)!;
            var redis = MockSkills.GetSkillById(MockSkills.RedisId)!;

            return [
                new UsersSkills { IdUsers = MockUsers.KirillId,     Skills = [ javaScript, kotlin, react, vue ] },
                new UsersSkills { IdUsers = MockUsers.IvanId,       Skills = [ python, rust, cpp, redis ]       },
                new UsersSkills { IdUsers = MockUsers.ManagerId,    Skills = [ javaScript, python, vue, redis ] },
                new UsersSkills 
                { 
                    IdUsers = MockUsers.OwnerId, 
                    Skills = [ python, MockSkills.GetSkillById(MockSkills.GoId)!, react, MockSkills.GetSkillById(MockSkills.MySQLId)! ]
                },
                new UsersSkills 
                { 
                    IdUsers = MockUsers.WinritId, 
                    Skills = [ python, rust, MockSkills.GetSkillById(MockSkills.SwiftId)!, MockSkills.GetSkillById(MockSkills.PostgreSQLId)! ]
                },
                new UsersSkills 
                { 
                    IdUsers = MockUsers.VersalId, 
                    Skills = [ javaScript, rust, cpp, MockSkills.GetSkillById(MockSkills.MongoDBId)! ] 
                },
                new UsersSkills 
                { 
                    IdUsers = MockUsers.AntonId, 
                    Skills = [ rust, cpp, MockSkills.GetSkillById(MockSkills.DockerId)! ] 
                }
            ];
        }

        public static List<Skill>? GetUserSkillsById(Guid userId)
            => _usersSkills.FirstOrDefault(us => us.IdUsers == userId)?.Skills;
    }
}
