using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using System.Xml.Linq;

namespace HITSBlazor.Utils.Mocks.Markets
{
    public static class MockMarkets
    {
        public static Guid Summer2023Id { get; } = Guid.NewGuid();
        public static Guid Autumn2023Id { get; } = Guid.NewGuid();
        public static Guid Winter2024Id { get; } = Guid.NewGuid();
        public static Guid Spring2024Id { get; } = Guid.NewGuid();

        private static readonly List<Market> _markets = CreateMarkets();

        private static List<Market> CreateMarkets() => 
        [
            new Market
            {
                Id = Summer2023Id,
                Name = "Летняя биржа 2023",
                StartDate = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                FinishDate = new DateTime(2023, 8, 31, 0, 0, 0, DateTimeKind.Utc),
                Status = MarketStatus.Done
            },
            new Market
            {
                Id = Autumn2023Id,
                Name = "Осенняя биржа 2023",
                StartDate = new DateTime(2023, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                FinishDate = new DateTime(2023, 11, 30, 0, 0, 0, DateTimeKind.Utc),
                Status = MarketStatus.Active
            },
            new Market
            {
                Id = Winter2024Id,
                Name = "Зимняя биржа 2024",
                StartDate = new DateTime(2023, 12, 1, 0, 0, 0, DateTimeKind.Utc),
                FinishDate = new DateTime(2024, 2, 28, 0, 0, 0, DateTimeKind.Utc),
                Status = MarketStatus.New
            },
            new Market
            {
                Id = Spring2024Id,
                Name = "Весенняя биржа 2024",
                StartDate = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                FinishDate = new DateTime(2024, 5, 31, 0, 0, 0, DateTimeKind.Utc),
                Status = MarketStatus.New
            }
        ];

        public static List<Market> GetAllMarkets() => [.. _markets];

        public static Market? GetMarketById(Guid marketId) => _markets.FirstOrDefault(m => m.Id == marketId);

        public static Market? CreateMarket(string name, DateTime startDate, DateTime finishDate, MarketStatus status = MarketStatus.New)
        {
            var market = new Market
            {
                Name = name,
                StartDate = startDate,
                FinishDate = finishDate,
                Status = status
            };

            _markets.Add(market);

            return market;
        }

        public static bool UpdateMarket(Guid marketId, string name, DateTime startDate, DateTime finishDate, MarketStatus status)
        {
            var marketForUpdate = _markets.FirstOrDefault(m => m.Id == marketId);
            if (marketForUpdate is null) return false;

            marketForUpdate.Name = name;
            marketForUpdate.StartDate = startDate;
            marketForUpdate.FinishDate = finishDate;
            marketForUpdate.Status = status;

            return true;
        }

        public static bool UpdateMarketStatus(Guid marketId, MarketStatus status)
        {
            var marketForUpdate = _markets.FirstOrDefault(m => m.Id == marketId);
            if (marketForUpdate is null) return false;

            marketForUpdate.Status = status;

            return true;
        }

        public static bool DeleteMarket(Market market) => _markets.Remove(market);
    }
}
