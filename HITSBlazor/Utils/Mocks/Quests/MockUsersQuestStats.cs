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

        // измеить моки, сделать тут участников команд

        private static List<UsersQuestStat> CreateUsersQuestStats() =>
        [
            CreateUsersQuestStat(MockUsers.GetUserById(MockUsers.KirillId)!, false),
            CreateUsersQuestStat(MockUsers.GetUserById(MockUsers.IvanId)!, false),
            CreateUsersQuestStat(MockUsers.GetUserById(MockUsers.ManagerId)!, false),
            CreateUsersQuestStat(MockUsers.GetUserById(MockUsers.OwnerId)!, true)
        ];
    }
}
