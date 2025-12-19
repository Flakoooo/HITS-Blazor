using HITSBlazor.Models.Quests.Entities;

namespace HITSBlazor.Utils.Mocks.Quests
{
    public static class MockIndicatorCategories
    {
        private static readonly List<IndicatorCategory> _indicatorCategories = CreateIndicatorCategories();

        public static Guid SoftId { get; } = Guid.NewGuid();
        public static Guid HardId { get; } = Guid.NewGuid();

        private static List<IndicatorCategory> CreateIndicatorCategories() =>
        [
            new IndicatorCategory { IdCategory = SoftId, Name = "soft" },
            new IndicatorCategory { IdCategory = HardId, Name = "hard" }
        ];

        public static IndicatorCategory? GetIndicatorCategoryById(Guid id) =>
            _indicatorCategories.FirstOrDefault(ic => ic.IdCategory == id);
    }
}
