using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Common
{
    public static class MockNotifications
    {
        private static readonly List<Notification> _notifications = CreateNotifications();

        public static Guid Project1KirillId { get; } = Guid.NewGuid();
        public static Guid Project2TimurId { get; } = Guid.NewGuid();
        public static Guid Project1DenisId { get; } = Guid.NewGuid();

        private static List<Notification> CreateNotifications()
        {
            return
            [
                new Notification
                {
                    Id = Project1KirillId,
                    UserId = MockUsers.KirillId,
                    Title = "Чат 1",
                    Message = "Князев(менеджер, проект 1): дедлайн завтра",
                    Link = null,
                    IsShowed = true,
                    IsReaded = false,
                    IsFavorite = false,
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat)
                },
                new Notification
                {
                    Id = Project2TimurId,
                    UserId = MockUsers.TimurId,
                    Title = "Чат 2",
                    Message = "Иванов(менеджер, проект 2): дедлайн завтра",
                    Link = null,
                    IsShowed = false,
                    IsReaded = false,
                    IsFavorite = false,
                    CreatedAt = new DateTime(2023, 10, 28, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat)
                },
                new Notification
                {
                    Id = Project1DenisId,
                    UserId = MockUsers.DenisId,
                    Title = "Чат 1",
                    Message = "Князев(менеджер, проект 1): дедлайн завтра",
                    Link = null,
                    IsShowed = false,
                    IsReaded = true,
                    IsFavorite = true,
                    CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat)
                },
            ];
        }

        public static List<Notification> GetAllNotificationsByUserId(Guid userId)
            => [.. _notifications.Where(n => n.UserId == userId)];

        public static List<Notification> GetReadedNotificationsByUserId(Guid userId)
            => [.. _notifications.Where(n => n.UserId == userId && n.IsReaded)];

        public static List<Notification> GetUnreadedNotificationsByUserId(Guid userId)
            => [.. _notifications.Where(n => n.UserId == userId && !n.IsReaded)];

        public static List<Notification> GetFavoriteNotificationsByUserId(Guid userId)
            => [.. _notifications.Where(n => n.UserId == userId && n.IsFavorite)];
    }
}
