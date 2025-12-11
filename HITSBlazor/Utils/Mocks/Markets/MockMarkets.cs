using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;

namespace HITSBlazor.Utils.Mocks.Markets
{
    public static class MockMarkets
    {
        private static readonly List<Market> _markets = CreateMarkets();

        public static string Summer2023Id { get; } = Guid.NewGuid().ToString();
        public static string Autumn2023Id { get; } = Guid.NewGuid().ToString();
        public static string Winter2024Id { get; } = Guid.NewGuid().ToString();
        public static string Spring2024Id { get; } = Guid.NewGuid().ToString();

        

        private static List<Market> CreateMarkets() => 
        [
            new Market
            {
                Id = Summer2023Id,
                Name = "Летняя биржа 2023",
                StartDate = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc).ToString(Settings.DateFormat),
                FinishDate = new DateTime(2023, 8, 31, 0, 0, 0, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Status = MarketStatus.DONE
            },
            new Market
            {
                Id = Autumn2023Id,
                Name = "Осенняя биржа 2023",
                StartDate = new DateTime(2023, 9, 1, 0, 0, 0, DateTimeKind.Utc).ToString(Settings.DateFormat),
                FinishDate = new DateTime(2023, 11, 30, 0, 0, 0, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Status = MarketStatus.ACTIVE
            },
            new Market
            {
                Id = Winter2024Id,
                Name = "Зимняя биржа 2024",
                StartDate = new DateTime(2023, 12, 1, 0, 0, 0, DateTimeKind.Utc).ToString(Settings.DateFormat),
                FinishDate = new DateTime(2024, 2, 28, 0, 0, 0, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Status = MarketStatus.NEW
            },
            new Market
            {
                Id = Spring2024Id,
                Name = "Весенняя биржа 2024",
                StartDate = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc).ToString(Settings.DateFormat),
                FinishDate = new DateTime(2024, 5, 31, 0, 0, 0, DateTimeKind.Utc).ToString(Settings.DateFormat),
                Status = MarketStatus.NEW
            }
        ];
    }
}
