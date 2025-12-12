using HITSBlazor.Models.Quests.Entities;

namespace HITSBlazor.Utils.Mocks.Quests
{
    public static class MockIndicatorCategories
    {
        private static readonly List<IndicatorCategory> _indicatorCategories = CreateIndicatorCategories();

        public static string SoftId { get; } = Guid.NewGuid().ToString();
        public static string HardId { get; } = Guid.NewGuid().ToString();

        private static List<IndicatorCategory> CreateIndicatorCategories() =>
        [
            new IndicatorCategory { IdCategory = SoftId, Name = "soft" },
            new IndicatorCategory { IdCategory = HardId, Name = "hard" }
        ];

        public static IndicatorCategory? GetIndicatorCategoryById(string id) =>
            _indicatorCategories.FirstOrDefault(ic => ic.IdCategory == id);
    }
}
