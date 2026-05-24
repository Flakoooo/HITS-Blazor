using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Utils.Mocks.Markets;
using static HITSBlazor.Utils.Mocks.Common.MockInvitation;

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

        public static ListDataResponse<RequestTeamToIdea> GetRequestsTeamToIdeas(
            int page,
            int pageSize = 20,
            Guid? teamId = null,
            Guid? ideaMarketId = null,
            string? searchText = null
        )
        {
            IQueryable<RequestTeamToIdea> query;

            query = (teamId, ideaMarketId) switch
            {
                (null, null) => _requestTeamToIdeas.AsQueryable(),
                (null, _) => _requestTeamToIdeas.Where(rtti => rtti.IdeaMarketId == ideaMarketId).AsQueryable(),
                (_, null) => _requestTeamToIdeas.Where(rtti => rtti.TeamId == teamId).AsQueryable(),
                (_, _) => _requestTeamToIdeas.Where(rtti => rtti.TeamId == teamId && rtti.IdeaMarketId == ideaMarketId).AsQueryable()
            };

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(rtti => rtti.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            if (teamId.HasValue)
            {
                var list = query.ToList().Select(rtti =>
                {
                    rtti.Name = (teamId, ideaMarketId) switch
                    {
                        (null, _) => MockTeams.GetTeamById(rtti.TeamId)?.Name,
                        (_, null) => MockIdeaMarkets.GetIdeaMarketById(rtti.IdeaMarketId)?.Name,
                        (_, _) => "ОШИБКА"
                    } ?? "ОШИБКА";
                    return rtti;
                }).ToList();

                return new ListDataResponse<RequestTeamToIdea>(count, list);
            }
            else
            {
                return new ListDataResponse<RequestTeamToIdea>(count, query.ToList());
            }
        }

        public static List<RequestTeamToIdea> GetTeamRequestForCurrentTeamsAndIdeaMarket(
            Guid ideaMarketId,
            HashSet<Guid> teamIds
        ) => _requestTeamToIdeas.Where(rtti => rtti.IdeaMarketId == ideaMarketId && teamIds.Contains(rtti.TeamId)).ToList();

        public static RequestTeamToIdea CreateNewRequest(IdeaMarket ideaMarket, Team team, string letter)
        {
            var request = new RequestTeamToIdea
            {
                Id = Guid.NewGuid(),
                IdeaMarketId = ideaMarket.Id,
                MarketId = ideaMarket.MarketId,
                TeamId = team.Id,
                Status = TeamRequestStatus.New,
                Name = team.Name,
                MembersCount = team.MembersCount,
                Skills = team.Skills,
                Letter = letter
            };

            _requestTeamToIdeas.Add(request);

            return request;
        }

        public static void AnnulledRequestByTeamId(Guid teamId)
        {
            foreach (var request in _requestTeamToIdeas.Where(r => r.TeamId == teamId))
                request.Status = TeamRequestStatus.Annulled;
        }

        public static bool UpdateStatus(Guid requestId, TeamRequestStatus newStatus)
        {
            var request = _requestTeamToIdeas.FirstOrDefault(r => r.Id == requestId);
            if (request is null) return false;

            if (newStatus is TeamRequestStatus.Accepted)
            {
                var acceptedTeam = MockTeams.GetTeamById(request.TeamId);
                var ideaMarket = MockIdeaMarkets.GetIdeaMarketById(request.IdeaMarketId);

                if (acceptedTeam is null || ideaMarket is null) return false;

                acceptedTeam.IsAcceptedToIdea = true;

                ideaMarket.Team = acceptedTeam;
                ideaMarket.Status = IdeaMarketStatusType.RecruitmentIsClosed;

                AnnulledRequestByTeamId(request.TeamId);
                MockInvitationTeamToIdeas.AnnulledInvitationByTeamId(request.TeamId);
            }

            request.Status = newStatus;

            return true;
        }
    }
}
