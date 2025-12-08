using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Utils.Mocks.Users
{
    public static class MockUserTelegrams
    {
        public static List<UserTelegram> GetMockUserTelegrams() => [
            new UserTelegram { UserId = MockUsers.KirillId, UserTag = "baobao",         ChatId = "0",   IsVisible = true    },
            new UserTelegram { UserId = MockUsers.IvanId,   UserTag = "@chipichipi",    ChatId = "5",   IsVisible = false   },
            new UserTelegram { UserId = MockUsers.IvanId,   UserTag = "@chapachapa",    ChatId = null,  IsVisible = true    }
        ];

        public static UserTelegram? GetUserTelegramByUserId(string userId)
            => GetMockUserTelegrams().FirstOrDefault(ut => ut.UserId == userId);

        public static List<UserTelegram> GetVisibleUserTelegrams()
            => [.. GetMockUserTelegrams().Where(ut => ut.IsVisible)];

        public static UserTelegram? GetUserTelegramByTag(string userTag)
            => GetMockUserTelegrams().FirstOrDefault(ut => ut.UserTag == userTag);
    }
}
