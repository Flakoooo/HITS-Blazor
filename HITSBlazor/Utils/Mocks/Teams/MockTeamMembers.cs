using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockTeamMembers
    {
        public static string KirillId { get; } = Guid.NewGuid().ToString();
        public static string TimurId { get; } = Guid.NewGuid().ToString();
        public static string AdminId { get; } = Guid.NewGuid().ToString();
        public static string DenisId { get; } = Guid.NewGuid().ToString();

        public static List<TeamMember> GetMockTeamMembers()
        {
            User kirill = MockUsers.GetUserById(MockUsers.KirillId);
            User timur = MockUsers.GetUserById(MockUsers.TimurId);
            User admin = MockUsers.GetUserById(MockUsers.AdminId);
            User denis = MockUsers.GetUserById(MockUsers.DenisId);

            return
            [
                new TeamMember
                {
                    Id = TimurId,
                    TeamId = MockTeams.CardId,
                    UserId = MockUsers.TimurId,
                    Email = timur.Email,
                    FirstName = timur.FirstName,
                    LastName = timur.LastName,
                    Skills = []
                },
                new TeamMember
                {
                    Id = KirillId,
                    TeamId = MockTeams.CardId,
                    UserId = MockUsers.KirillId,
                    Email = kirill.Email,
                    FirstName = kirill.FirstName,
                    LastName = kirill.LastName,
                    Skills = []
                },
                new TeamMember
                {
                    Id = AdminId,
                    TeamId = "0",
                    UserId = MockUsers.AdminId,
                    Email = admin.Email,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Skills = []
                },
                new TeamMember
                {
                    Id = DenisId,
                    TeamId = MockTeams.CardId,
                    UserId = MockUsers.DenisId,
                    Email = denis.Email,
                    FirstName = denis.FirstName,
                    LastName = denis.LastName,
                    Skills = []
                }
            ];
        }

        public static TeamMember? GetTeamMemberById(string id)
            => GetMockTeamMembers().FirstOrDefault(tm => tm.Id == id);

        public static List<TeamMember> GetTeamMembersByTeamId(string teamId)
            => [.. GetMockTeamMembers().Where(tm => tm.TeamId == teamId)];

        public static List<TeamMember> GetTeamMembersByUserId(string userId)
            => [.. GetMockTeamMembers().Where(tm => tm.UserId == userId)];

        public static TeamMember? GetTeamMemberByEmail(string email)
            => GetMockTeamMembers().FirstOrDefault(tm => tm.Email == email);

        public static string GetFullName(this TeamMember teamMember)
            => $"{teamMember.FirstName} {teamMember.LastName}";
    }
}
