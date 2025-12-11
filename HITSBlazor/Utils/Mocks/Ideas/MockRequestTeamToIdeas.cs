using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Utils.Mocks.Markets;
using HITSBlazor.Utils.Mocks.Teams;

namespace HITSBlazor.Utils.Mocks.Ideas
{
    public class MockRequestTeamToIdeas
    {
        private static readonly List<RequestTeamToIdea> requestTeamToIdeas = CreateRequestTeamToIdeas();

        private static readonly string _lorem = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Dolorbeatae ipsum dicta omnis adipisci magni autem eos quisquam doloresmaxime. Dignissimos cum nulla consequatur accusantium distinctioaut. Velit, assumenda porro!";

        public static string Card1Id { get; } = Guid.NewGuid().ToString();
        public static string Card2Id { get; } = Guid.NewGuid().ToString();
        public static string Card3Id { get; } = Guid.NewGuid().ToString();
        public static string Cactus1Id { get; } = Guid.NewGuid().ToString();
        public static string Cactus2Id { get; } = Guid.NewGuid().ToString();
        public static string Carp1Id { get; } = Guid.NewGuid().ToString();
        public static string Carp2Id { get; } = Guid.NewGuid().ToString();
        public static string Carp3Id { get; } = Guid.NewGuid().ToString();

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
                    Status = RequestToIdeaStatus.ANNULLED,
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
                    Status = RequestToIdeaStatus.ANNULLED,
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
                    Status = RequestToIdeaStatus.ANNULLED,
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
                    Status = RequestToIdeaStatus.ACCEPTED,
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
                    Status = RequestToIdeaStatus.ACCEPTED,
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
                    Status = RequestToIdeaStatus.WITHDRAWN,
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
                    Status = RequestToIdeaStatus.CANCELED,
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
                    Status = RequestToIdeaStatus.NEW,
                    Name = carpTeam.Name,
                    MembersCount = carpTeam.MembersCount,
                    Skills = carpTeam.Skills,
                    Letter = _lorem
                }
            ];
        }
    }
}
