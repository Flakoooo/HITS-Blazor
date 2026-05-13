using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
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
                Members = [.. MockUsers.GetUsersByRole(RoleType.Initiator)],
                Roles = [RoleType.Initiator]
            },
            new UsersGroup
            {
                Id = ProjectOfficeId,
                Name = "Группа проектного офиса",
                Members = [.. MockUsers.GetUsersByRole(RoleType.ProjectOffice)],
                Roles = [RoleType.ProjectOffice]
            },
            new UsersGroup
            {
                Id = ExpertsId,
                Name = "Группа экспертов",
                Members = [.. MockUsers.GetUsersByRole(RoleType.Expert)],
                Roles = [RoleType.Expert]
            }
        ];

        public static ListDataResponse<UsersGroup> GetAllGroups(
            int page,
            int pageSize = 20,
            string? searchText = null,
            HashSet<RoleType>? selectedRoles = null
        )
        {
            var query = _usersGroups.AsEnumerable();

            if (selectedRoles?.Count > 0)
                query = query.Where(ug => ug.Roles.Any(selectedRoles.Contains));

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(ug => ug.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new ListDataResponse<UsersGroup>(count, query.ToList());
        }

        public static UsersGroup? GetGroupById(Guid id)
            => _usersGroups.FirstOrDefault(g => g.Id == id);

        public static ListDataResponse<User> GetUsersGroupMembers(
            Guid usersGroupId,
            int page,
            int pageSize = 20,
            string? searchText = null
        )
        {
            var usersGroup = _usersGroups.FirstOrDefault(ug => ug.Id == usersGroupId);
            if (usersGroup is null) return new ListDataResponse<User>(0, []);

            var query = usersGroup.Members.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(ug => ug.FullName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) 
                    || ug.Email.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)
                );

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new ListDataResponse<User>(count, query.ToList());
        }

        public static List<User> GetRandomGroupUsersById(Guid id, int userCount)
        {
            var users = _usersGroups.FirstOrDefault(g => g.Id == id)!.Members;

            var usedIndexes = new HashSet<int>();
            while (usedIndexes.Count < userCount)
                usedIndexes.Add(_random.Next(users.Count));

            return usedIndexes.Select(i => users[i]).ToList();
        }

        public static UsersGroup? CreateUsersGroup(string name, List<Guid> membersIds, List<RoleType> roles)
        {
            var members = new List<User>();
            foreach (var memberId in membersIds)
            {
                var member = MockUsers.GetUserById(memberId);
                if (member is not null) members.Add(member);
            }

            var usersGroup = new UsersGroup
            {
                Name = name,
                Members = members,
                Roles = roles
            };

            _usersGroups.Add(usersGroup);

            return usersGroup;
        }

        public static UsersGroup? UpdateUsersGroup(
            Guid usersGroupId, 
            string? name = null, 
            IEnumerable<Guid>? newMembersIds = null,
            HashSet<Guid>? removeMembersIds = null,
            IEnumerable<RoleType>? roles = null
        )
        {
            var usersGroup = _usersGroups.FirstOrDefault(ug => ug.Id == usersGroupId);
            if (usersGroup is null) return usersGroup;

            if (name is not null) usersGroup.Name = name;

            if (newMembersIds is not null)
            {
                foreach (var memberId in newMembersIds)
                {
                    var member = MockUsers.GetUserById(memberId);
                    if (member is not null) usersGroup.Members.Add(member);
                }
            }

            if (removeMembersIds is not null)
            {
                usersGroup.Members.RemoveAll(u => removeMembersIds.Contains(u.Id));
            }

            if (roles is not null)
                usersGroup.Roles = roles.ToList();

            return usersGroup;
        }

        public static bool DeleteUsersGroup(UsersGroup usersGroup) => _usersGroups.Remove(usersGroup);
    }
}
