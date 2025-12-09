using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Common
{
    public static class MockUsersGroups
    {
        public static string DevelopersId { get; } = Guid.NewGuid().ToString();
        public static string ExpertsId { get; } = Guid.NewGuid().ToString();

        public static List<UsersGroup> GetMockUsersGroups() => [
            new UsersGroup
            {
                Id = DevelopersId,
                Name = "Группа разработчиков",
                Users = [.. MockUsers.GetMockUsers()],
                Roles = [RoleType.INITIATOR]
            },
            new UsersGroup
            {
                Id = ExpertsId,
                Name = "Группа экспертов",
                Users = [],
                Roles = [RoleType.ADMIN, RoleType.EXPERT]
            }
        ];

        public static UsersGroup? GetGroupById(string id)
            => GetMockUsersGroups().FirstOrDefault(g => g.Id == id);

        public static UsersGroup? GetGroupByName(string name)
            => GetMockUsersGroups().FirstOrDefault(g => g.Name == name);

        public static List<UsersGroup> GetGroupsByUser(string userId)
            => [.. GetMockUsersGroups().Where(g => g.Users.Any(u => u.Id == userId))];

        public static List<UsersGroup> GetGroupsByRole(RoleType role)
            => [.. GetMockUsersGroups().Where(g => g.Roles.Contains(role))];

        public static List<UsersGroup> GetGroupsWithUsers()
            => [.. GetMockUsersGroups().Where(g => g.Users.Any())];

        public static List<UsersGroup> GetEmptyGroups()
            => [.. GetMockUsersGroups().Where(g => !g.Users.Any())];

        public static bool IsUserInGroup(string userId, string groupId)
        {
            var group = GetGroupById(groupId);
            return group?.Users.Any(u => u.Id == userId) ?? false;
        }

        public static bool GroupHasRole(string groupId, RoleType role)
        {
            var group = GetGroupById(groupId);
            return group?.Roles.Contains(role) ?? false;
        }
    }
}
