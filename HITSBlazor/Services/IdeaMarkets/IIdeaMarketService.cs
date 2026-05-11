using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Teams.Entities;

namespace HITSBlazor.Services.IdeaMarkets
{
    public interface IIdeaMarketService
    {
        Task<ListDataResponse<IdeaMarket>> GetIdeasMarketAsync(
            int page,
            Guid? marketId = null,
            bool? favorite = null,
            string? searchText = null,
            IdeaMarketStatusType? selectedStatus = null
        );

        Task<IdeaMarket?> GetIdeaMarketAsync(Guid guid);
        Task<List<RequestTeamToIdea>> GetRequestsTeamToIdeaAsync(Guid ideaMarketId, string? searchText = null);
        Task<List<InvitationTeamToIdea>> GetInvitationTeamsToIdeaAsync(Guid ideaId, string? searchText = null);

        Task<bool> SendIdeasOnMarket(ICollection<Idea> ideas, Market market);

        Task<bool> SetIdeaFavorite(Guid userId, IdeaMarket ideaMarket);
        Task<bool> UnsetIdeaFromFavorite(Guid userId, IdeaMarket ideaMarket);
    }
}
