using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Markets
{
    public static class MockIdeaMarketAdvertisements
    {
        public static string BackendOnlyAdId { get; } = Guid.NewGuid().ToString();
        public static string ClosingSoonAdId { get; } = Guid.NewGuid().ToString();
        public static string NeedFrontendBackendAdId { get; } = Guid.NewGuid().ToString();

        public static List<IdeaMarketAdvertisement> GetMockIdeaMarketAdvertisements()
        {
            User manager = MockUsers.GetUserById(MockUsers.ManagerId);
            User kirill = MockUsers.GetUserById(MockUsers.KirillId);

            return
            [
                new IdeaMarketAdvertisement
                {
                    Id = BackendOnlyAdId,
                    IdeaMarketId = MockIdeaMarkets.HelperId, 
                    CreatedAt = "2023-03-11T11:02:17Z",
                    Text = "Для выполнения данной идеи требуются только бекендеры!",
                    Sender = manager,
                    CheckedBy = [manager.Email]
                },
                new IdeaMarketAdvertisement
                {
                    Id = ClosingSoonAdId,
                    IdeaMarketId = MockIdeaMarkets.HelperId,
                    CreatedAt = "2023-03-12T11:02:17Z",
                    Text = "Скоро набор закроется, быстрее подавайте заявки!",
                    Sender = manager,
                    CheckedBy = [manager.Email]
                },
                new IdeaMarketAdvertisement
                {
                    Id = NeedFrontendBackendAdId,
                    IdeaMarketId = MockIdeaMarkets.PWTechnologyId,
                    CreatedAt = "2023-03-18T11:02:17Z",
                    Text = "Требуются фронтендеры, бекендеры и желающие научиться новым компетенциям.",
                    Sender = kirill,
                    CheckedBy = [kirill.Email]
                }
            ];
        }

        public static IdeaMarketAdvertisement? GetAdvertisementById(string id)
            => GetMockIdeaMarketAdvertisements().FirstOrDefault(a => a.Id == id);

        public static List<IdeaMarketAdvertisement> GetAdvertisementsByIdeaMarketId(string ideaMarketId)
            => [.. GetMockIdeaMarketAdvertisements().Where(a => a.IdeaMarketId == ideaMarketId)];

        public static List<IdeaMarketAdvertisement> GetAdvertisementsBySender(string senderId)
            => [.. GetMockIdeaMarketAdvertisements().Where(a => a.Sender.Id == senderId)];

        public static List<IdeaMarketAdvertisement> GetAdvertisementsCheckedByUser(string userEmail)
            => [.. GetMockIdeaMarketAdvertisements().Where(a => a.CheckedBy.Contains(userEmail))];

        public static List<IdeaMarketAdvertisement> GetRecentAdvertisements(int count = 5)
            => [.. GetMockIdeaMarketAdvertisements()
                  .OrderByDescending(a => a.CreatedAt)
                  .Take(count)];

        public static bool IsAdvertisementCheckedByUser(string advertisementId, string userEmail)
        {
            var advertisement = GetAdvertisementById(advertisementId);
            return advertisement?.CheckedBy.Contains(userEmail) ?? false;
        }

        public static void MarkAsChecked(string advertisementId, string userEmail)
        {
            var advertisement = GetAdvertisementById(advertisementId);
            if (advertisement != null && !advertisement.CheckedBy.Contains(userEmail))
            {
                advertisement.CheckedBy.Add(userEmail);
            }
        }

        public static int GetCheckCount(string advertisementId)
            => GetAdvertisementById(advertisementId)?.CheckedBy.Count ?? 0;
    }
}
