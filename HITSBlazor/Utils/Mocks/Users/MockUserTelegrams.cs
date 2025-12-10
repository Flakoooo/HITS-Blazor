using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Utils.Mocks.Users
{
    public static class MockUserTelegrams
    {
        private static readonly List<UserTelegram> _telegramUsers = CreateTelegramUsers();

        private static List<UserTelegram> CreateTelegramUsers() => [
            new UserTelegram { UserId = MockUsers.KirillId, UserTag = "baobao",         ChatId = "0",   IsVisible = true    },
            new UserTelegram { UserId = MockUsers.IvanId,   UserTag = "@chipichipi",    ChatId = "5",   IsVisible = false   },
            new UserTelegram { UserId = MockUsers.IvanId,   UserTag = "@chapachapa",    ChatId = null,  IsVisible = true    }
        ];
    }
}
