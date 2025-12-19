using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Utils.Mocks.Users
{
    public static class MockUsers
    {
        private static List<string>? _cachedEmails;
        private static readonly List<User> _users = CreateUsers();

        public static List<string> CachedUserEmails => _cachedEmails ??= GetUserEmails();

        public static Guid KirillId { get; } = Guid.NewGuid();
        public static Guid IvanId { get; } = Guid.NewGuid();
        public static Guid ManagerId { get; } = Guid.NewGuid();
        public static Guid OwnerId { get; } = Guid.NewGuid();
        public static Guid WinritId { get; } = Guid.NewGuid();
        public static Guid VersalId { get; } = Guid.NewGuid();
        public static Guid AntonId { get; } = Guid.NewGuid();
        public static Guid LubovId { get; } = Guid.NewGuid();
        public static Guid DmitryId { get; } = Guid.NewGuid();
        public static Guid TimurId { get; } = Guid.NewGuid();
        public static Guid AdminId { get; } = Guid.NewGuid();
        public static Guid DenisId { get; } = Guid.NewGuid();
        public static Guid MagaId { get; } = Guid.NewGuid();
        public static Guid AlexId { get; } = Guid.NewGuid();

        private static List<User> CreateUsers() => [ 
            new User
            {
                Id = KirillId,
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
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Telephone = "1111111111",
                StudyGroup = "AAAA-22-1"
            },
            new User
            {
                Id = IvanId,
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
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Telephone = "22222222222",
                StudyGroup = "BBBB-22-1"
            },
            new User
            {
                Id = ManagerId,
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
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Telephone = "33333333333",
                StudyGroup = "CCCC-22-1"
            },
            new User
            {
                Id = OwnerId,
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
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Telephone = "44444444444",
                StudyGroup = "DDDD-22-1"
            },
            new User
            {
                Id = WinritId,
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
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Telephone = "55555555555",
                StudyGroup = "EEEE-22-1"
            },
            new User
            {
                Id = VersalId,
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
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Telephone = "66666666666",
                StudyGroup = "FFFF-22-1"
            },
            new User
            {
                Id = AntonId,
                Token = "861478",
                Email = "warkingzar@gmail.com",
                FirstName = "Антон",
                LastName = "Зайко",
                Roles =
                [
                    RoleType.INITIATOR,
                    RoleType.PROJECT_OFFICE,
                    RoleType.EXPERT,
                    RoleType.ADMIN,
                    RoleType.MEMBER
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Telephone = "66667776666",
                StudyGroup = "GGGG-22-1"
            },
            new User
            {
                Id = LubovId,
                Token = "890578",
                Email = "l.a.nikiforova@tmn3.etagi.com",
                FirstName = "Любовь",
                LastName = "Никифорова",
                Roles =
                [
                    RoleType.INITIATOR,
                    RoleType.PROJECT_OFFICE,
                    RoleType.EXPERT,
                    RoleType.ADMIN,
                    RoleType.MEMBER
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Telephone = "66933776666",
                StudyGroup = "HHHH-22-1"
            },
            new User
            {
                Id = DmitryId,
                Token = "890954",
                Email = "d.shirokov@unlim.group",
                FirstName = "Дмитрий",
                LastName = "Широков",
                Roles =
                [
                    RoleType.INITIATOR,
                    RoleType.PROJECT_OFFICE,
                    RoleType.EXPERT,
                    RoleType.ADMIN,
                    RoleType.MEMBER
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Telephone = "6699856666",
                StudyGroup = "IIII-22-1"
            },
            new User
            {
                Id = TimurId,
                Token = "890923",
                Email = "timyr@mail.com",
                FirstName = "Тимур",
                LastName = "Минязев",
                Roles =
                [
                    RoleType.INITIATOR,
                    RoleType.PROJECT_OFFICE,
                    RoleType.EXPERT,
                    RoleType.ADMIN,
                    RoleType.MEMBER
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Telephone = "6699856666",
                StudyGroup = "AAAA-22-1"
            },
            new User
            {
                Id = AdminId,
                Token = "864923",
                Email = "admin@mail.com",
                FirstName = "Админ",
                LastName = "Иванов",
                Roles =
                [
                    RoleType.INITIATOR,
                    RoleType.PROJECT_OFFICE,
                    RoleType.EXPERT,
                    RoleType.ADMIN,
                    RoleType.MEMBER
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Telephone = "6699986666",
                StudyGroup = "JJJJ-22-1"
            },
            new User
            {
                Id = DenisId,
                Token = "864103",
                Email = "deins@mail.com",
                FirstName = "Денис",
                LastName = "Денисович",
                Roles =
                [
                    RoleType.INITIATOR,
                    RoleType.PROJECT_OFFICE,
                    RoleType.EXPERT,
                    RoleType.ADMIN,
                    RoleType.MEMBER
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Telephone = "6699986726",
                StudyGroup = "KKKK-22-1"
            },
            new User
            {
                Id = MagaId,
                Token = "704103",
                Email = "maga@mail.com",
                FirstName = "Мамедага",
                LastName = "Байрамов",
                Roles =
                [
                    RoleType.INITIATOR,
                    RoleType.PROJECT_OFFICE,
                    RoleType.EXPERT,
                    RoleType.ADMIN,
                    RoleType.MEMBER
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Telephone = "6699986789",
                StudyGroup = "AAAA-22-1"
            },
            new User
            {
                Id = AlexId,
                Token = "709803",
                Email = "alex@inbox.ru",
                FirstName = "Алексей",
                LastName = "Князев",
                Roles =
                [
                    RoleType.INITIATOR,
                    RoleType.PROJECT_OFFICE,
                    RoleType.EXPERT,
                    RoleType.ADMIN,
                    RoleType.MEMBER
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Telephone = "6699986789",
                StudyGroup = "LLLL-22-1"
            }
        ];

        public static User? GetUserById(Guid id)
            => _users.FirstOrDefault(u => u.Id == id);

        public static List<string> GetUserEmails() 
            => [.. _users.Select(u => u.Email)];

        public static List<User> GetAllUsers() => _users;
    }
}
