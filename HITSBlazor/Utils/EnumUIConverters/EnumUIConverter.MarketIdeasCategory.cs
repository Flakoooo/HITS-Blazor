using HITSBlazor.Pages.Markets.MarketIdeas;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetMarketIdeasCategoryInfo(MarketIdeasCategory category) => category switch
        {
            MarketIdeasCategory.All => new EnumUIResult("Все", string.Empty),
            MarketIdeasCategory.Favorite => new EnumUIResult("Избранное", string.Empty),
            _ => new EnumUIResult(category.ToString(), string.Empty)
        };
    }
}
