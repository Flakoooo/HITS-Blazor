using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Common
{
    public static class MockUsersGroups
    {
        private static readonly List<UsersGroup> _usersGroups = CreateUsersGroups();

        public static Guid DevelopersId { get; } = Guid.NewGuid();
        public static Guid ExpertsId { get; } = Guid.NewGuid();

        private static List<UsersGroup> CreateUsersGroups() => [
            new UsersGroup
            {
                Id = DevelopersId,
                Name = "Группа разработчиков",
                Users = [.. MockUsers.GetAllUsers()],
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

        public static UsersGroup? GetGroupById(Guid id)
            => _usersGroups.FirstOrDefault(g => g.Id == id);
    }
}
