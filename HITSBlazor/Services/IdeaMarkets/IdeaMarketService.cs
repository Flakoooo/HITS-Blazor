using ApexCharts;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Users;
using HITSBlazor.Utils.Mocks.Markets;

namespace HITSBlazor.Services.IdeaMarkets
{
    public class IdeaMarketService(
        IAuthService authService,
        IdeaMarketApi ideaMarketApi,
        ILogger<IdeaMarketService> logger,
        GlobalNotificationService globalNotificationService
    ) : IIdeaMarketService
    {
        private readonly IAuthService _authService = authService;
        private readonly IdeaMarketApi _ideaMarketApi = ideaMarketApi;
        private readonly ILogger<IdeaMarketService> _logger = logger;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;


        public async Task<ListDataResponse<IdeaMarket>> GetIdeasMarketAsync(
            int page,
            Guid? marketId,
            bool? favorite,
            string? searchText,
            IdeaMarketStatusType? selectedStatus
        )
        {
            var result = await _ideaMarketApi.GetIdeaMarketsAsync(
                page,
                marketId: marketId,
                favorite: favorite,
                searchText: searchText,
                selectedStatus: selectedStatus
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get market ideas failed: {Error}", result.Message);
            }

            return new ListDataResponse<IdeaMarket>(0, []);
        }

        public async Task<IdeaMarket?> GetIdeaMarketAsync(Guid guid)
        {
            var result = await _ideaMarketApi.GetIdeaMarketAsync(guid);

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get market idea failed: {Error}", result.Message);
            }

            return result.Response;
        }

        public async Task<bool> SendIdeasOnMarket(ICollection<Idea> ideas, Market market)
        {
            var result = await _ideaMarketApi.CreateIdeaMarketsAsync(market.Id, ideas);

            if (result.IsSuccess && result.Response is not null)
            {
                _globalNotificationService.ShowSuccess(result.Response);
            }
            else if (!result.IsSuccess)
            {
                _globalNotificationService.ShowError("Не удалось отправить идеи на биржу");
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Create market idea failed: {Error}", result.Message);
            }

            return result.IsSuccess;
        }

        public async Task<bool> SetIdeaFavorite(IdeaMarket ideaMarket)
        {
            var result = await _ideaMarketApi.SetIdeaMarketFavoriteAsync(ideaMarket.Id);
            if (result.IsSuccess && result.Response is not null)
            {
                _globalNotificationService.ShowSuccess(result.Response);
            }
            else if (!result.IsSuccess && !string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Set market idea favorite failed: {Error}", result.Message);
            }

            return result.IsSuccess;
        }

        public async Task<bool> UnsetIdeaFromFavorite(IdeaMarket ideaMarket)
        {
            var result = await _ideaMarketApi.UnsetIdeaMarketFavoriteAsync(ideaMarket.Id);
            if (result.IsSuccess && result.Response is not null)
            {
                _globalNotificationService.ShowSuccess(result.Response);
            }
            else if (!result.IsSuccess && !string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Unset market idea favorite failed: {Error}", result.Message);
            }

            return result.IsSuccess;
        }
    }
}
