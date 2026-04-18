using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Ideas;
using HITSBlazor.Utils.Mocks.Teams;
using System.Collections;

namespace HITSBlazor.Utils.Mocks.Markets
{
    public static class MockIdeaMarkets
    {
        private class FavoriteIdeas
        {
            public Guid UserId { get; set; }
            public Guid IdeaMarketId { get; set; }

            public override int GetHashCode() => UserId.GetHashCode();
            public override bool Equals(object? obj)
                => obj is FavoriteIdeas fi && UserId == fi.UserId && IdeaMarketId == fi.IdeaMarketId;
        }

        public static Guid HelperId { get; } = Guid.NewGuid();
        public static Guid PWTechnologyId { get; } = Guid.NewGuid();
        public static Guid EMetricsViewerId { get; } = Guid.NewGuid();
        public static Guid CalculatorId { get; } = Guid.NewGuid();
        public static Guid ChatBotId { get; } = Guid.NewGuid();
        public static Guid ArmatureId { get; } = Guid.NewGuid();

        private static readonly List<IdeaMarket> _ideaMarkets = CreateIdeaMarkets();
        private static readonly HashSet<FavoriteIdeas> _favoriteIdeas = [];

        private static List<IdeaMarket> CreateIdeaMarkets()
        {
            var helper = MockIdeas.GetIdeaById(MockIdeas.HelperId)!;
            var pwTechnology = MockIdeas.GetIdeaById(MockIdeas.PWTechnologyId)!;
            var eMetricsViewer = MockIdeas.GetIdeaById(MockIdeas.EMetricsViewerId)!;
            var calculator = MockIdeas.GetIdeaById(MockIdeas.CalculatorId)!;
            var chatbot = MockIdeas.GetIdeaById(MockIdeas.ChatBotId)!;
            var armature = MockIdeas.GetIdeaById(MockIdeas.ArmatureId)!;

            var python = MockSkills.GetSkillById(MockSkills.PythonId)!;
            var postgreSQL = MockSkills.GetSkillById(MockSkills.PostgreSQLId)!;
            var docker = MockSkills.GetSkillById(MockSkills.DockerId)!;
            var git = MockSkills.GetSkillById(MockSkills.GitId)!;

            var stackAI = new List<Skill>() 
            { 
                python, 
                MockSkills.GetSkillById(MockSkills.KerasId)!, 
                MockSkills.GetSkillById(MockSkills.ScikitLearnId)!, 
                postgreSQL, 
                docker 
            };
            var stackWeb = new List<Skill>() 
            { 
                python, 
                docker, 
                MockSkills.GetSkillById(MockSkills.GoId)!, 
                MockSkills.GetSkillById(MockSkills.CSharpId)!, 
                MockSkills.GetSkillById(MockSkills.VueId)!, 
                git, 
                MockSkills.GetSkillById(MockSkills.RedisId)!, 
                postgreSQL 
            };

            return 
            [
                new IdeaMarket
                {
                    Id = HelperId,
                    IdeaId = helper.Id,
                    Initiator = helper.Initiator,
                    Team = null,
                    MarketId = MockMarkets.Autumn2023Id,
                    Name = helper.Name,
                    Problem = helper.Problem,
                    Description = helper.Description,
                    Solution = helper.Solution,
                    Result = helper.Result,
                    MaxTeamSize = helper.MaxTeamSize,
                    Customer = helper.Customer,
                    Position = 7,
                    Stack = [..stackAI],
                    Status = IdeaMarketStatusType.RecruitmentIsOpen,
                    Requests = 0,
                    AcceptedRequests = 0,
                    IsFavorite = false
                },
                new IdeaMarket
                {
                    Id = PWTechnologyId,
                    IdeaId = pwTechnology.Id,
                    Initiator = pwTechnology.Initiator,
                    Team = null,
                    MarketId = MockMarkets.Autumn2023Id,
                    Name = pwTechnology.Name,
                    Problem = pwTechnology.Problem,
                    Description = pwTechnology.Description,
                    Solution = pwTechnology.Solution,
                    Result = pwTechnology.Result,
                    MaxTeamSize = pwTechnology.MaxTeamSize,
                    Customer = pwTechnology.Customer,
                    Position = 8,
                    Stack =
                    [
                        MockSkills.GetSkillById(MockSkills.UnrealEngineId)!,
                        git,
                        MockSkills.GetSkillById(MockSkills.CppId)!
                    ],
                    Status = IdeaMarketStatusType.RecruitmentIsOpen,
                    Requests = 0,
                    AcceptedRequests = 0,
                    IsFavorite = false
                },
                new IdeaMarket
                {
                    Id = EMetricsViewerId,
                    IdeaId = eMetricsViewer.Id,
                    Initiator = eMetricsViewer.Initiator,
                    Team = null,
                    MarketId = MockMarkets.Autumn2023Id,
                    Name = eMetricsViewer.Name,
                    Problem = eMetricsViewer.Problem,
                    Description = eMetricsViewer.Description,
                    Solution = eMetricsViewer.Solution,
                    Result = eMetricsViewer.Result,
                    MaxTeamSize = eMetricsViewer.MaxTeamSize,
                    Customer = eMetricsViewer.Customer,
                    Position = 9,
                    Stack = [..stackWeb],
                    Status = IdeaMarketStatusType.RecruitmentIsOpen,
                    Requests = 0,
                    AcceptedRequests = 0,
                    IsFavorite = false
                },
                new IdeaMarket
                {
                    Id = CalculatorId,
                    IdeaId = calculator.Id,
                    Initiator = calculator.Initiator,
                    Team = null,
                    MarketId = MockMarkets.Autumn2023Id,
                    Name = calculator.Name,
                    Problem = calculator.Problem,
                    Description = calculator.Description,
                    Solution = calculator.Solution,
                    Result = calculator.Result,
                    MaxTeamSize = calculator.MaxTeamSize,
                    Customer = calculator.Customer,
                    Position = 10,
                    Stack = [.. stackWeb],
                    Status = IdeaMarketStatusType.RecruitmentIsOpen,
                    Requests = 0,
                    AcceptedRequests = 0,
                    IsFavorite = false
                },
                new IdeaMarket
                {
                    Id = ChatBotId,
                    IdeaId = chatbot.Id,
                    Initiator = chatbot.Initiator,
                    Team = MockTeams.GetTeamById(MockTeams.CardId),
                    MarketId = MockMarkets.Autumn2023Id,
                    Name = chatbot.Name,
                    Problem = chatbot.Problem,
                    Description = chatbot.Description,
                    Solution = chatbot.Solution,
                    Result = chatbot.Result,
                    MaxTeamSize = chatbot.MaxTeamSize,
                    Customer = chatbot.Customer,
                    Position = 11,
                    Stack = [.. stackAI],
                    Status = IdeaMarketStatusType.RecruitmentIsClosed,
                    Requests = 0,
                    AcceptedRequests = 0,
                    IsFavorite = false
                },
                new IdeaMarket
                {
                    Id = ArmatureId,
                    IdeaId = armature.Id,
                    Initiator = armature.Initiator,
                    Team = MockTeams.GetTeamById(MockTeams.CactusId),
                    MarketId = MockMarkets.Autumn2023Id,
                    Name = armature.Name,
                    Problem = armature.Problem,
                    Description = armature.Description,
                    Solution = armature.Solution,
                    Result = armature.Result,
                    MaxTeamSize = armature.MaxTeamSize,
                    Customer = armature.Customer,
                    Position = 11,
                    Stack = [.. stackAI],
                    Status = IdeaMarketStatusType.Project,
                    Requests = 0,
                    AcceptedRequests = 0,
                    IsFavorite = false
                }
            ];
        }

        private static List<IdeaMarket> GetIdeaMarkets(Guid userId, Func<IdeaMarket, bool> predicate)
        {
            var favoriteIds = _favoriteIdeas
                .Where(f => f.UserId == userId)
                .Select(f => f.IdeaMarketId)
                .ToHashSet();

            return [.. _ideaMarkets
                .Where(predicate)
                .Select(im => new IdeaMarket
                {
                    Id = im.Id,
                    IdeaId = im.IdeaId,
                    Initiator = im.Initiator,
                    Team = im.Team,
                    MarketId = im.MarketId,
                    Name = im.Name,
                    Problem = im.Problem,
                    Description = im.Description,
                    Solution = im.Solution,
                    Result = im.Result,
                    MaxTeamSize = im.MaxTeamSize,
                    Customer = im.Customer,
                    Position = im.Position,
                    Stack = [.. im.Stack],
                    Status = im.Status,
                    Requests = im.Requests,
                    AcceptedRequests = im.AcceptedRequests,
                    IsFavorite = favoriteIds.Contains(im.Id)
                })];
        }

        public static List<IdeaMarket> GetIdeaMarketsByMarketId(Guid marketId, Guid userId)
            => GetIdeaMarkets(userId, im => im.MarketId == marketId);

        public static List<IdeaMarket> GetIdeaMarketsByMarketIdAndInitiatorId(Guid marketId, Guid initiatorId)
            => GetIdeaMarkets(initiatorId, im => im.MarketId == marketId && im.Initiator.Id == initiatorId);

        public static IdeaMarket? GetIdeaMarketById(Guid id) =>
            _ideaMarkets.FirstOrDefault(im => im.Id == id);

        public static int SendIdeasOnMarket(ICollection<Idea> ideas, Market market)
        {
            int count = 0;
            foreach (var idea in ideas)
            {
                if (_ideaMarkets.Any(im => im.IdeaId == idea.Id && im.MarketId == market.Id))
                    continue;

                _ideaMarkets.Add(new IdeaMarket
                {
                    Id = Guid.NewGuid(),
                    IdeaId = idea.Id,
                    Initiator = idea.Initiator,
                    Team = null,
                    MarketId = market.Id,
                    Name = idea.Name,
                    Problem = idea.Problem,
                    Description = idea.Description,
                    Solution = idea.Solution,
                    Result = idea.Result,
                    MaxTeamSize = idea.MaxTeamSize,
                    Customer = idea.Customer,
                    Position = 0,
                    Stack = MockIdeaSkills.GetIdeaSkillsByIdeaId(idea.Id),
                    Status = IdeaMarketStatusType.RecruitmentIsOpen,
                    Requests = 0,
                    AcceptedRequests = 0,
                    IsFavorite = false
                });
                idea.Status = IdeaStatusType.OnMarket;
                ++count;
            }

            return count;
        }

        public static bool SetIdeaFavorite(Guid userId, Guid ideaMarketId)
            => _favoriteIdeas.Add(new FavoriteIdeas { UserId = userId, IdeaMarketId = ideaMarketId });

        public static bool UnsetIdeaFromFavorite(Guid userId, Guid ideaMarketId)
        {
            var favorite = _favoriteIdeas.FirstOrDefault(f => f.UserId == userId && f.IdeaMarketId == ideaMarketId);
            if (favorite is null) return false;

            return _favoriteIdeas.Remove(favorite);
        }
    }
}
