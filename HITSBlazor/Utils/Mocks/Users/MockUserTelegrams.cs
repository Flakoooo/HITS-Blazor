using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Utils.Mocks.Users
{
    public static class MockUserTelegrams
    {
        private static readonly List<UserTelegram> _telegramUsers = CreateTelegramUsers();

        private static List<UserTelegram> CreateTelegramUsers() => [
            new UserTelegram { UserId = MockUsers.KirillId, UserTag = "baobao",         ChatId = Guid.NewGuid(),   IsVisible = true    },
            new UserTelegram { UserId = MockUsers.IvanId,   UserTag = "@chipichipi",    ChatId = Guid.NewGuid(),   IsVisible = false   },
            new UserTelegram { UserId = MockUsers.IvanId,   UserTag = "@chapachapa",    ChatId = Guid.Empty,  IsVisible = true    }
        ];

        public static UserTelegram? GetTelegramInfoByUserId(Guid userId)
            => _telegramUsers.FirstOrDefault(ut => ut.UserId == userId);
    }
}
