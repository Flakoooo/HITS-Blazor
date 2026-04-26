using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Models.Users.Requests;

namespace HITSBlazor.Utils.Mocks.Users
{
    public static class MockUsers
    {
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

        private static List<string>? _cachedEmails;
        private static readonly List<User> _users = CreateUsers();

        public static List<string> CachedUserEmails => _cachedEmails ??= GetUserEmails();

        private static List<User> CreateUsers() => [ 
            new User
            {
                Id = KirillId,
                Email = "kirill.vlasov.05@inbox.ru",
                FirstName = "Кирилл",
                LastName = "Власов",
                Roles =
                [
                    RoleType.Initiator,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Member,
                    RoleType.TeamLeader,
                    RoleType.Teacher
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                Telephone = "1111111111",
                StudyGroup = "AAAA-22-1"
            },
            new User
            {
                Id = IvanId,
                Email = "1@mail.com",
                FirstName = "Иван",
                LastName = "Иванович",
                Roles =
                [
                    RoleType.Initiator,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Member,
                    RoleType.Teacher
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                Telephone = "22222222222",
                StudyGroup = "BBBB-22-1"
            },
            new User
            {
                Id = ManagerId,
                Email = "2@mail.com",
                FirstName = "Менеджер",
                LastName = "Менеджер",
                Roles =
                [
                    RoleType.Initiator,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Member,
                    RoleType.Teacher
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                Telephone = "33333333333",
                StudyGroup = "CCCC-22-1"
            },
            new User
            {
                Id = OwnerId,
                Email = "3@mail.com",
                FirstName = "Владелец",
                LastName = "Владелец",
                Roles =
                [
                    RoleType.Initiator,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Member,
                    RoleType.Teacher
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                Telephone = "44444444444",
                StudyGroup = "DDDD-22-1"
            },
            new User
            {
                Id = WinritId,
                Email = "4@mail.com",
                FirstName = "Винрит",
                LastName = "Загрев",
                Roles =
                [
                    RoleType.Initiator,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Member
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                Telephone = "55555555555",
                StudyGroup = "EEEE-22-1"
            },
            new User
            {
                Id = VersalId,
                Email = "5@mail.com",
                FirstName = "Версаль",
                LastName = "Кустерман",
                Roles =
                [
                    RoleType.Initiator,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Member
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                Telephone = "66666666666",
                StudyGroup = "FFFF-22-1"
            },
            new User
            {
                Id = AntonId,
                Email = "warkingzar@gmail.com",
                FirstName = "Антон",
                LastName = "Зайко",
                Roles =
                [
                    RoleType.Initiator,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Member
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                Telephone = "66667776666",
                StudyGroup = "GGGG-22-1"
            },
            new User
            {
                Id = LubovId,
                Email = "l.a.nikiforova@tmn3.etagi.com",
                FirstName = "Любовь",
                LastName = "Никифорова",
                Roles =
                [
                    RoleType.Initiator,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Member
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                Telephone = "66933776666",
                StudyGroup = "HHHH-22-1"
            },
            new User
            {
                Id = DmitryId,
                Email = "d.shirokov@unlim.group",
                FirstName = "Дмитрий",
                LastName = "Широков",
                Roles =
                [
                    RoleType.Initiator,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Member
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                Telephone = "6699856666",
                StudyGroup = "IIII-22-1"
            },
            new User
            {
                Id = TimurId,
                Email = "timyr@mail.com",
                FirstName = "Тимур",
                LastName = "Минязев",
                Roles =
                [
                    RoleType.Initiator,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Member
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                Telephone = "6699856666",
                StudyGroup = "AAAA-22-1"
            },
            new User
            {
                Id = AdminId,
                Email = "admin@mail.com",
                FirstName = "Админ",
                LastName = "Иванов",
                Roles =
                [
                    RoleType.Initiator,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Member
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                Telephone = "6699986666",
                StudyGroup = "JJJJ-22-1"
            },
            new User
            {
                Id = DenisId,
                Email = "deins@mail.com",
                FirstName = "Денис",
                LastName = "Денисович",
                Roles =
                [
                    RoleType.Initiator,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Member
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                Telephone = "6699986726",
                StudyGroup = "KKKK-22-1"
            },
            new User
            {
                Id = MagaId,
                Email = "maga@mail.com",
                FirstName = "Мамедага",
                LastName = "Байрамов",
                Roles =
                [
                    RoleType.Initiator,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Member
                ],
                CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                Telephone = "6699986789",
                StudyGroup = "AAAA-22-1"
            },
            new User
            {
                Id = AlexId,
                Email = "alex@inbox.ru",
                FirstName = "Алексей",
                LastName = "Князев",
                Roles =
                [
                    RoleType.Initiator,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Member
                ],
                CreatedAt = new DateTime(2025, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                Telephone = "6699986789",
                StudyGroup = "LLLL-22-1"
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "blazortext@gmail.ru",
                FirstName = "<h1>Заголовок</h1>",
                LastName = "'>\"><h1>кавычки</h1>",
                Roles =
                [
                    RoleType.Initiator,
                    RoleType.ProjectOffice,
                    RoleType.Expert,
                    RoleType.Admin,
                    RoleType.Member
                ],
                CreatedAt = new DateTime(2026, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                Telephone = "6699986789",
                StudyGroup = "LLLL-22-1"
            }
        ];

        public static List<User> GetAllMockUsers() => _users;

        public static ListDataResponse<User> GetAllUsers(
            int page,
            int pageSize = 20,
            string? searchText = null,
            string? orderBy = null,
            bool? byDescending = null,
            HashSet<RoleType>? selectedRoles = null
        )
        {
            var query = _users.AsEnumerable();

            if (selectedRoles?.Count > 0)
                query = query.Where(u => u.Roles.Any(selectedRoles.Contains));

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(u => u.FullName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(orderBy) && byDescending.HasValue)
            {
                query = (orderBy, byDescending.Value) switch
                {
                    (nameof(User.CreatedAt), true) => query.OrderByDescending(u => u.CreatedAt),
                    (nameof(User.CreatedAt), false) => query.OrderBy(u => u.CreatedAt),
                    _ => query
                };
            }

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new ListDataResponse<User> { Count = count, List = query.ToList() };
        }

        public static User? GetUserById(Guid id)
            => _users.FirstOrDefault(u => u.Id == id);

        public static List<string> GetUserEmails() 
            => [.. _users.Select(u => u.Email)];

        public static List<User> GetUsersByRole(RoleType role) 
            => [.. _users.Where(u => u.Roles.Contains(role))];

        public static User? UpdateUser(UpdateUserRequest updatedUser)
        {
            var user = _users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (user is null) return null;

            user.Email = updatedUser.Email;
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Telephone = updatedUser.Telephone;
            user.StudyGroup = updatedUser.StudyGroup;

            return user;
        }

        public static bool DeleteUser(User user) => _users.Remove(user);
    }
}
