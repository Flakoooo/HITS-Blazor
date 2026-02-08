using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Utils.Mocks.Markets;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public class MockRequestTeamToIdeas
    {
        private static readonly string _lorem = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Dolorbeatae ipsum dicta omnis adipisci magni autem eos quisquam doloresmaxime. Dignissimos cum nulla consequatur accusantium distinctioaut. Velit, assumenda porro!";

        public static Guid Card1Id { get; } = Guid.NewGuid();
        public static Guid Card2Id { get; } = Guid.NewGuid();
        public static Guid Card3Id { get; } = Guid.NewGuid();
        public static Guid Cactus1Id { get; } = Guid.NewGuid();
        public static Guid Cactus2Id { get; } = Guid.NewGuid();
        public static Guid Carp1Id { get; } = Guid.NewGuid();
        public static Guid Carp2Id { get; } = Guid.NewGuid();
        public static Guid Carp3Id { get; } = Guid.NewGuid();

        private static readonly List<RequestTeamToIdea> _requestTeamToIdeas = CreateRequestTeamToIdeas();

        private static List<RequestTeamToIdea> CreateRequestTeamToIdeas()
        {
            var cardTeam = MockTeams.GetTeamById(MockTeams.CardId)!;
            var cactusTeam = MockTeams.GetTeamById(MockTeams.CactusId)!;
            var carpTeam = MockTeams.GetTeamById(MockTeams.CarpId)!;

            return [
                new RequestTeamToIdea
                {
                    Id = Card1Id,
                    IdeaMarketId = MockIdeaMarkets.EMetricsViewerId,
                    MarketId = MockMarkets.Autumn2023Id,
                    TeamId = cardTeam.Id,
                    Status = TeamRequestStatus.Annulled,
                    Name = cardTeam.Name,
                    MembersCount = cardTeam.MembersCount,
                    Skills = cardTeam.Skills,
                    Letter = _lorem
                },
                new RequestTeamToIdea
                {
                    Id = Card2Id,
                    IdeaMarketId = MockIdeaMarkets.PWTechnologyId,
                    MarketId = MockMarkets.Autumn2023Id,
                    TeamId = cardTeam.Id,
                    Status = TeamRequestStatus.Annulled,
                    Name = cardTeam.Name,
                    MembersCount = cardTeam.MembersCount,
                    Skills = cardTeam.Skills,
                    Letter = _lorem
                },
                new RequestTeamToIdea
                {
                    Id = Cactus1Id,
                    IdeaMarketId = MockIdeaMarkets.PWTechnologyId,
                    MarketId = MockMarkets.Autumn2023Id,
                    TeamId = cactusTeam.Id,
                    Status = TeamRequestStatus.Annulled,
                    Name = cactusTeam.Name,
                    MembersCount = cactusTeam.MembersCount,
                    Skills = cactusTeam.Skills,
                    Letter = _lorem
                },
                new RequestTeamToIdea
                {
                    Id = Card3Id,
                    IdeaMarketId = MockIdeaMarkets.ChatBotId,
                    MarketId = MockMarkets.Autumn2023Id,
                    TeamId = cardTeam.Id,
                    Status = TeamRequestStatus.Accepted,
                    Name = cardTeam.Name,
                    MembersCount = cardTeam.MembersCount,
                    Skills = cardTeam.Skills,
                    Letter = _lorem
                },
                new RequestTeamToIdea
                {
                    Id = Cactus2Id,
                    IdeaMarketId = MockIdeaMarkets.ArmatureId,
                    MarketId = MockMarkets.Autumn2023Id,
                    TeamId = cactusTeam.Id,
                    Status = TeamRequestStatus.Accepted,
                    Name = cactusTeam.Name,
                    MembersCount = cactusTeam.MembersCount,
                    Skills = cactusTeam.Skills,
                    Letter = _lorem
                },
                new RequestTeamToIdea
                {
                    Id = Carp1Id,
                    IdeaMarketId = MockIdeaMarkets.ArmatureId,
                    MarketId = MockMarkets.Autumn2023Id,
                    TeamId = carpTeam.Id,
                    Status = TeamRequestStatus.Withdrawn,
                    Name = carpTeam.Name,
                    MembersCount = carpTeam.MembersCount,
                    Skills = carpTeam.Skills,
                    Letter = _lorem
                },
                new RequestTeamToIdea
                {
                    Id = Carp2Id,
                    IdeaMarketId = MockIdeaMarkets.HelperId,
                    MarketId = MockMarkets.Autumn2023Id,
                    TeamId = carpTeam.Id,
                    Status = TeamRequestStatus.Canceled,
                    Name = carpTeam.Name,
                    MembersCount = carpTeam.MembersCount,
                    Skills = carpTeam.Skills,
                    Letter = _lorem
                },
                new RequestTeamToIdea
                {
                    Id = Carp3Id,
                    IdeaMarketId = MockIdeaMarkets.PWTechnologyId,
                    MarketId = MockMarkets.Autumn2023Id,
                    TeamId = carpTeam.Id,
                    Status = TeamRequestStatus.New,
                    Name = carpTeam.Name,
                    MembersCount = carpTeam.MembersCount,
                    Skills = carpTeam.Skills,
                    Letter = _lorem
                }
            ];
        }

        public static List<RequestTeamToIdea> GetRequestsTeamToIdeas(Guid teamId)
            => [.. _requestTeamToIdeas.Where(rtti => rtti.TeamId == teamId)];
    }
}
