using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Utils.Mock.Users
{
    public static class MockUsers
    {
        public static List<User> GetMockUsers()
        {
            return
            [
                new User
                {
                    Id = "ffc1b25e-8a65-4cb2-8808-6eba443acec8",
                    Token = "10296538",
                    Email = "kirill.vlasov.05@inbox.ru",
                    FirstName = "Кирилл",
                    LastName = "Власов",
                    Roles =
                    [
                        RoleType.INITIATOR,
                        RoleType.PROJECT_OFFICE,
                        RoleType.EXPERT,
                        RoleType.ADMIN,
                        RoleType.MEMBER,
                        RoleType.TEAM_LEADER,
                        RoleType.TEACHER
                    ],
                    CreatedAt = "2023-10-20T11:02:17Z",
                    Telephone = "1111111111",
                    StudyGroup = "AAAA-22-1"
                },
                new User
                {
                    Id = "126288eb-9d4d-4074-9c87-6e4a566ef8f9",
                    Token = "1",
                    Email = "1@mail.com",
                    FirstName = "Иван",
                    LastName = "Иванович",
                    Roles =
                    [
                        RoleType.INITIATOR,
                        RoleType.PROJECT_OFFICE,
                        RoleType.EXPERT,
                        RoleType.ADMIN,
                        RoleType.MEMBER,
                        RoleType.TEACHER
                    ],
                    CreatedAt = "2023-10-20T11:02:17Z",
                    Telephone = "22222222222",
                    StudyGroup = "BBBB-22-1"
                },
                new User
                {
                    Id = "2",
                    Token = "059182",
                    Email = "2@mail.com",
                    FirstName = "Менеджер",
                    LastName = "Менеджер",
                    Roles =
                    [
                        RoleType.INITIATOR,
                        RoleType.PROJECT_OFFICE,
                        RoleType.EXPERT,
                        RoleType.ADMIN,
                        RoleType.MEMBER,
                        RoleType.TEACHER
                    ],
                    CreatedAt = "2023-10-20T11:02:17Z",
                    Telephone = "33333333333",
                    StudyGroup = "CCCC-22-1"
                },
                new User
                {
                    Id = "3",
                    Token = "163097",
                    Email = "3@mail.com",
                    FirstName = "Владелец",
                    LastName = "Владелец",
                    Roles =
                    [
                        RoleType.INITIATOR,
                        RoleType.PROJECT_OFFICE,
                        RoleType.EXPERT,
                        RoleType.ADMIN,
                        RoleType.MEMBER,
                        RoleType.TEACHER
                    ],
                    CreatedAt = "2023-10-20T11:02:17Z",
                    Telephone = "44444444444",
                    StudyGroup = "DDDD-22-1"
                },
                new User
                {
                    Id = "4",
                    Token = "8755764",
                    Email = "4@mail.com",
                    FirstName = "Винрит",
                    LastName = "Загрев",
                    Roles =
                    [
                        RoleType.INITIATOR,
                        RoleType.PROJECT_OFFICE,
                        RoleType.EXPERT,
                        RoleType.ADMIN,
                        RoleType.MEMBER
                    ],
                    CreatedAt = "2023-10-20T11:02:17Z",
                    Telephone = "55555555555",
                    StudyGroup = "EEEE-22-1"
                },
                new User
                {
                    Id = "5",
                    Token = "836444",
                    Email = "5@mail.com",
                    FirstName = "Версаль",
                    LastName = "Кустерман",
                    Roles =
                    [
                        RoleType.INITIATOR,
                        RoleType.PROJECT_OFFICE,
                        RoleType.EXPERT,
                        RoleType.ADMIN,
                        RoleType.MEMBER
                    ],
                    CreatedAt = "2023-10-20T11:02:17Z",
                    Telephone = "66666666666",
                    StudyGroup = "FFFF-22-1"
                }
            ];
        }

        public static User GetUserById(string id)
            => GetMockUsers().FirstOrDefault(u => u.Id == id) ?? new User();

        public static User GetUserByEmail(string email)
            => GetMockUsers().FirstOrDefault(u => u.Email == email) ?? new User();

        public static List<User> GetUsersByRole(RoleType role)
            => [.. GetMockUsers().Where(u => u.Roles.Contains(role))];

        public static User GetCurrentUser()
            => GetMockUsers().First();
    }
}
