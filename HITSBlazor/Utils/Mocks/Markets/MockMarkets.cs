using HITSBlazor.Models.Common.Responses;
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
                StartDate = DateOnly.FromDateTime(new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc)),
                FinishDate = DateOnly.FromDateTime(new DateTime(2023, 8, 31, 0, 0, 0, DateTimeKind.Utc)),
                Status = MarketStatus.Done
            },
            new Market
            {
                Id = Autumn2023Id,
                Name = "Осенняя биржа 2023",
                StartDate = DateOnly.FromDateTime(new DateTime(2023, 9, 1, 0, 0, 0, DateTimeKind.Utc)),
                FinishDate = DateOnly.FromDateTime(new DateTime(2023, 11, 30, 0, 0, 0, DateTimeKind.Utc)),
                Status = MarketStatus.Active
            },
            new Market
            {
                Id = Winter2024Id,
                Name = "Зимняя биржа 2024",
                StartDate = DateOnly.FromDateTime(new DateTime(2023, 12, 1, 0, 0, 0, DateTimeKind.Utc)),
                FinishDate = DateOnly.FromDateTime(new DateTime(2024, 2, 28, 0, 0, 0, DateTimeKind.Utc)),
                Status = MarketStatus.New
            },
            new Market
            {
                Id = Spring2024Id,
                Name = "Весенняя биржа 2024",
                StartDate = DateOnly.FromDateTime(new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc)),
                FinishDate = DateOnly.FromDateTime(new DateTime(2024, 5, 31, 0, 0, 0, DateTimeKind.Utc)),
                Status = MarketStatus.New
            }
        ];

        public static ListDataResponse<Market> GetMarketsByQueryParams(
            int page,
            int pageSize = 20,
            string? searchText = null,
            HashSet<MarketStatus>? selectedStatuses = null,
            string? orderBy = null,
            bool? byDescending = null
        )
        {
            var query = _markets.AsEnumerable();

            if (selectedStatuses?.Count > 0)
                query = query.Where(m => selectedStatuses.Contains(m.Status));

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(m => m.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(orderBy) && byDescending.HasValue)
            {
                query = (orderBy, byDescending.Value) switch
                {
                    (nameof(Market.StartDate), true) => query.OrderByDescending(m => m.StartDate),
                    (nameof(Market.StartDate), false) => query.OrderBy(m => m.StartDate),
                    (nameof(Market.FinishDate), true) => query.OrderByDescending(m => m.FinishDate),
                    (nameof(Market.FinishDate), false) => query.OrderBy(m => m.FinishDate),
                    _ => query
                };
            }

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new ListDataResponse<Market>(count, query.ToList());
        }

        public static List<Market> GetAllActiveMarkets()
            => _markets.Where(m => m.Status is MarketStatus.Active).ToList();

        public static Market? GetMarketById(Guid marketId) => _markets.FirstOrDefault(m => m.Id == marketId);

        public static Market? CreateMarket(string name, DateOnly startDate, DateOnly finishDate, MarketStatus status = MarketStatus.New)
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

        public static Market? UpdateMarket(Guid marketId, string name, DateOnly startDate, DateOnly finishDate, MarketStatus status)
        {
            var marketForUpdate = _markets.FirstOrDefault(m => m.Id == marketId);
            if (marketForUpdate is null) return null;

            marketForUpdate.Name = name;
            marketForUpdate.StartDate = startDate;
            marketForUpdate.FinishDate = finishDate;
            marketForUpdate.Status = status;

            return marketForUpdate;
        }

        public static bool UpdateMarketStatus(Guid marketId, MarketStatus status)
        {
            var marketForUpdate = _markets.FirstOrDefault(m => m.Id == marketId);
            if (marketForUpdate is null) return false;

            marketForUpdate.Status = status;

            if (marketForUpdate.Status is MarketStatus.Done)
                MockIdeaMarkets.ReturnIdeasFromMarket(marketId);

            return true;
        }

        public static bool DeleteMarket(Market market) => _markets.Remove(market);
    }
}
