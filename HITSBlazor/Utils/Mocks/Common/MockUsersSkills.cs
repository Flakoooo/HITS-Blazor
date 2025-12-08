using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Common
{
    public static class MockUsersSkills
    {
        public static List<UsersSkills> GetMockUsersSkills() => [
            new UsersSkills
            {
                IdUsers = MockUsers.KirillId,
                Skills = [
                    MockSkills.GetSkillById(MockSkills.JavaScriptId),
                    MockSkills.GetSkillById(MockSkills.KotlinId),
                    MockSkills.GetSkillById(MockSkills.ReactId),
                    MockSkills.GetSkillById(MockSkills.VueId)
                ]
            },
            new UsersSkills
            {
                IdUsers = MockUsers.IvanId,
                Skills = [
                    MockSkills.GetSkillById(MockSkills.PythonId),
                    MockSkills.GetSkillById(MockSkills.RustId),
                    MockSkills.GetSkillById(MockSkills.CppId),
                    MockSkills.GetSkillById(MockSkills.RedisId)
                ]
            },
            new UsersSkills
            {
                IdUsers = MockUsers.ManagerId,
                Skills = [
                    MockSkills.GetSkillById(MockSkills.JavaScriptId),
                    MockSkills.GetSkillById(MockSkills.PythonId),
                    MockSkills.GetSkillById(MockSkills.VueId),
                    MockSkills.GetSkillById(MockSkills.RedisId)
                ]
            },
            new UsersSkills
            {
                IdUsers = MockUsers.OwnerId,
                Skills = [
                    MockSkills.GetSkillById(MockSkills.PythonId),
                    MockSkills.GetSkillById(MockSkills.GoId),
                    MockSkills.GetSkillById(MockSkills.ReactId),
                    MockSkills.GetSkillById(MockSkills.MySQLId)
                ]
            },
            new UsersSkills
            {
                IdUsers = MockUsers.WinritId,
                Skills = [
                    MockSkills.GetSkillById(MockSkills.PythonId),
                    MockSkills.GetSkillById(MockSkills.RustId),
                    MockSkills.GetSkillById(MockSkills.SwiftId),
                    MockSkills.GetSkillById(MockSkills.PostgreSQLId)
                ]
            },
            new UsersSkills
            {
                IdUsers = MockUsers.VersalId,
                Skills = [
                    MockSkills.GetSkillById(MockSkills.JavaScriptId),
                    MockSkills.GetSkillById(MockSkills.RustId),
                    MockSkills.GetSkillById(MockSkills.CppId),
                    MockSkills.GetSkillById(MockSkills.MongoDBId)
                ]
            },
            new UsersSkills
            {
                IdUsers = MockUsers.AntonId,
                Skills = [
                    MockSkills.GetSkillById(MockSkills.RustId),
                    MockSkills.GetSkillById(MockSkills.CppId),
                    MockSkills.GetSkillById(MockSkills.DockerId)
                ]
            }
        ];

        public static UsersSkills? GetUsersSkillsByUserId(string userId)
            => GetMockUsersSkills().FirstOrDefault(us => us.IdUsers == userId);

        public static List<Skill> GetSkillsByUserId(string userId)
            => GetUsersSkillsByUserId(userId)?.Skills ?? [];

        public static List<string> GetUserIdsBySkill(string skillId)
            => [.. GetMockUsersSkills()
                  .Where(us => us.Skills.Any(s => s.Id == skillId))
                  .Select(us => us.IdUsers)];

        public static List<string> GetUserIdsBySkillName(string skillName)
            => [.. GetMockUsersSkills()
                  .Where(us => us.Skills.Any(s => s.Name == skillName))
                  .Select(us => us.IdUsers)];

        public static List<string> GetUserIdsBySkillType(SkillType skillType)
            => [.. GetMockUsersSkills()
                  .Where(us => us.Skills.Any(s => s.Type == skillType))
                  .Select(us => us.IdUsers)];

        public static List<UsersSkills> GetUsersSkillsBySkill(string skillId)
            => [.. GetMockUsersSkills().Where(us => us.Skills.Any(s => s.Id == skillId))];

        public static int GetSkillCountForUser(string userId)
            => GetSkillsByUserId(userId).Count;

        public static bool UserHasSkill(string userId, string skillId)
            => GetSkillsByUserId(userId).Any(s => s.Id == skillId);

        public static bool UserHasSkillByName(string userId, string skillName)
            => GetSkillsByUserId(userId).Any(s => s.Name == skillName);
    }
}
