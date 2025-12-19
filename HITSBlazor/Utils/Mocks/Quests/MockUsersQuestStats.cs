using HITSBlazor.Models.Quests.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Quests
{
    public static class MockUsersQuestStats
    {
        private static readonly List<UsersQuestStat> _questStats = CreateUsersQuestStats();

        private static UsersQuestStat CreateUsersQuestStat(User user, bool progress) => new()
        {
            Id = user.Id,
            Name = $"{user.FirstName} {user.LastName}",
            Progress = progress
        };

        private static List<UsersQuestStat> CreateUsersQuestStats() =>
        [
            CreateUsersQuestStat(MockUsers.GetUserById(MockUsers.KirillId)!, false),
            CreateUsersQuestStat(MockUsers.GetUserById(MockUsers.DenisId)!, false),
            CreateUsersQuestStat(MockUsers.GetUserById(MockUsers.TimurId)!, false),
            CreateUsersQuestStat(MockUsers.GetUserById(MockUsers.AdminId)!, true)
        ];

        public static UsersQuestStat? GetUsersQuestStatById(Guid id) =>
            _questStats.FirstOrDefault(uqs => uqs.Id == id);
    }
}
