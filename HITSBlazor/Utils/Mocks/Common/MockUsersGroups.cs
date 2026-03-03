using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Common
{
    public static class MockUsersGroups
    {
        public static Guid DevelopersId { get; } = Guid.NewGuid();
        public static Guid ProjectOfficeId { get; } = Guid.NewGuid();
        public static Guid ExpertsId { get; } = Guid.NewGuid();

        private static readonly Random _random = new();
        private static readonly List<UsersGroup> _usersGroups = CreateUsersGroups();

        private static List<UsersGroup> CreateUsersGroups() => [
            new UsersGroup
            {
                Id = DevelopersId,
                Name = "Группа разработчиков",
                Users = [.. MockUsers.GetUsersByRole(RoleType.Initiator)],
                Roles = [RoleType.Initiator]
            },
            new UsersGroup
            {
                Id = ProjectOfficeId,
                Name = "Группа проектного офиса",
                Users = [.. MockUsers.GetUsersByRole(RoleType.ProjectOffice)],
                Roles = [RoleType.ProjectOffice]
            },
            new UsersGroup
            {
                Id = ExpertsId,
                Name = "Группа экспертов",
                Users = [.. MockUsers.GetUsersByRole(RoleType.Expert)],
                Roles = [RoleType.Expert]
            }
        ];

        public static UsersGroup? GetGroupById(Guid id)
            => _usersGroups.FirstOrDefault(g => g.Id == id);

        public static List<User> GetRandomGroupUsersById(Guid id, int userCount)
        {
            var users = _usersGroups.FirstOrDefault(g => g.Id == id)!.Users;

            var usedIndexes = new HashSet<int>();
            while (usedIndexes.Count < userCount)
                usedIndexes.Add(_random.Next(users.Count));

            return [.. usedIndexes.Select(i => users[i])];
        }
    }
}
