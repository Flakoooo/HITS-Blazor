using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;

namespace HITSBlazor.Utils.Mocks.Markets
{
    public static class MockMarkets
    {
        public static string Summer2023Id { get; } = Guid.NewGuid().ToString();
        public static string Autumn2023Id { get; } = Guid.NewGuid().ToString();
        public static string Winter2024Id { get; } = Guid.NewGuid().ToString();
        public static string Spring2024Id { get; } = Guid.NewGuid().ToString();

        public static List<Market> GetMockMarkets() => [
            new Market
            {
                Id = Summer2023Id,
                Name = "Летняя биржа 2023",
                StartDate = "2023-10-25T11:02:17Z",
                FinishDate = "2023-10-25T11:02:17Z",
                Status = MarketStatus.DONE
            },
            new Market
            {
                Id = Autumn2023Id,
                Name = "Осенняя биржа 2023",
                StartDate = "2023-10-25T11:02:17Z",
                FinishDate = "2023-10-25T11:02:17Z",
                Status = MarketStatus.ACTIVE
            },
            new Market
            {
                Id = Winter2024Id,
                Name = "Зимняя биржа 2024",
                StartDate = "2023-10-25T11:02:17Z",
                FinishDate = "2023-10-25T11:02:17Z",
                Status = MarketStatus.NEW
            },
            new Market
            {
                Id = Spring2024Id,
                Name = "Весенняя биржа 2024",
                StartDate = "2023-10-25T11:02:17Z",
                FinishDate = "2023-10-25T11:02:17Z",
                Status = MarketStatus.NEW
            }
        ];

        public static Market? GetMarketById(string id)
            => GetMockMarkets().FirstOrDefault(m => m.Id == id);

        public static List<Market> GetActiveMarkets()
            => [.. GetMockMarkets().Where(m => m.Status == MarketStatus.ACTIVE)];

        public static List<Market> GetNewMarkets()
            => [.. GetMockMarkets().Where(m => m.Status == MarketStatus.NEW)];

        public static List<Market> GetCompletedMarkets()
            => [.. GetMockMarkets().Where(m => m.Status == MarketStatus.DONE)];

        public static Market? GetMarketByName(string name)
            => GetMockMarkets().FirstOrDefault(m => m.Name == name);

        public static Market GetCurrentActiveMarket()
            => GetMockMarkets().FirstOrDefault(m => m.Status == MarketStatus.ACTIVE)
               ?? GetMockMarkets().First();

        public static Market? GetMarketByYearAndSeason(int year, string season)
        {
            return GetMockMarkets().FirstOrDefault(m =>
                m.Name.Contains(season.ToLower(), StringComparison.CurrentCultureIgnoreCase) &&
                m.Name.Contains(year.ToString())
            );
        }

        public static List<Market> GetMarketsByYear(int year)
            => [.. GetMockMarkets().Where(m => m.Name.Contains(year.ToString()))];

    }
}
