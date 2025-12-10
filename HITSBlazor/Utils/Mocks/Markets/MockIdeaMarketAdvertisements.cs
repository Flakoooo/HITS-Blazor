using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Markets
{
    public static class MockIdeaMarketAdvertisements
    {
        private static readonly List<IdeaMarketAdvertisement> _ideaMarketAdvertisements = CreateIdeaMarketAdvertisements();

        public static string BackendOnlyAdId { get; } = Guid.NewGuid().ToString();
        public static string ClosingSoonAdId { get; } = Guid.NewGuid().ToString();
        public static string NeedFrontendBackendAdId { get; } = Guid.NewGuid().ToString();

        private static List<IdeaMarketAdvertisement> CreateIdeaMarketAdvertisements()
        {
            var manager = MockUsers.GetUserById(MockUsers.ManagerId)!;
            var kirill = MockUsers.GetUserById(MockUsers.KirillId)!;
            
            return
            [
                new IdeaMarketAdvertisement
                {
                    Id = BackendOnlyAdId,
                    IdeaMarketId = MockIdeaMarkets.HelperId, 
                    CreatedAt = new DateTime(2023, 3, 11, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Text = "Для выполнения данной идеи требуются только бекендеры!",
                    Sender = manager,
                    CheckedBy = [manager.Email]
                },
                new IdeaMarketAdvertisement
                {
                    Id = ClosingSoonAdId,
                    IdeaMarketId = MockIdeaMarkets.HelperId,
                    CreatedAt = new DateTime(2023, 3, 12, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Text = "Скоро набор закроется, быстрее подавайте заявки!",
                    Sender = manager,
                    CheckedBy = [manager.Email]
                },
                new IdeaMarketAdvertisement
                {
                    Id = NeedFrontendBackendAdId,
                    IdeaMarketId = MockIdeaMarkets.PWTechnologyId,
                    CreatedAt = new DateTime(2023, 3, 18, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Text = "Требуются фронтендеры, бекендеры и желающие научиться новым компетенциям.",
                    Sender = kirill,
                    CheckedBy = [kirill.Email]
                }
            ];
        }
    }
}
