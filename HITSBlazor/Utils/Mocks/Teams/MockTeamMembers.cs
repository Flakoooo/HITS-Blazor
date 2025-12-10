using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockTeamMembers
    {
        private static readonly List<TeamMember> _teamMembers = CreateTeamMembers();

        public static string KirillId { get; } = Guid.NewGuid().ToString();
        public static string TimurId { get; } = Guid.NewGuid().ToString();
        public static string AdminId { get; } = Guid.NewGuid().ToString();
        public static string DenisId { get; } = Guid.NewGuid().ToString();

        private static List<TeamMember> CreateTeamMembers()
        {
            var kirill = MockUsers.GetUserById(MockUsers.KirillId)!;
            var timur = MockUsers.GetUserById(MockUsers.TimurId)!;
            var admin = MockUsers.GetUserById(MockUsers.AdminId)!;
            var denis = MockUsers.GetUserById(MockUsers.DenisId)!;

            return
            [
                new TeamMember
                {
                    Id = TimurId,
                    TeamId = MockTeams.CactusId,
                    UserId = timur.Id,
                    Email = timur.Email,
                    FirstName = timur.FirstName,
                    LastName = timur.LastName,
                    Skills = []
                },
                new TeamMember
                {
                    Id = KirillId,
                    TeamId = MockTeams.CardId,
                    UserId = kirill.Id,
                    Email = kirill.Email,
                    FirstName = kirill.FirstName,
                    LastName = kirill.LastName,
                    Skills = []
                },
                new TeamMember
                {
                    Id = AdminId,
                    TeamId = MockTeams.CactusId,
                    UserId = admin.Id,
                    Email = admin.Email,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Skills = []
                },
                new TeamMember
                {
                    Id = DenisId,
                    TeamId = MockTeams.CardId,
                    UserId = denis.Id,
                    Email = denis.Email,
                    FirstName = denis.FirstName,
                    LastName = denis.LastName,
                    Skills = []
                }
            ];
        }

        public static TeamMember? GetTeamMemberById(string id)
            => _teamMembers.FirstOrDefault(tm => tm.Id == id);
    }
}
