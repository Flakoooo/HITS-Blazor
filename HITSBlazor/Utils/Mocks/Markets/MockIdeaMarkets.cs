using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Ideas;
using HITSBlazor.Utils.Mocks.Teams;

namespace HITSBlazor.Utils.Mocks.Markets
{
    public static class MockIdeaMarkets
    {
        public static Guid HelperId { get; } = Guid.NewGuid();
        public static Guid PWTechnologyId { get; } = Guid.NewGuid();
        public static Guid EMetricsViewerId { get; } = Guid.NewGuid();
        public static Guid CalculatorId { get; } = Guid.NewGuid();
        public static Guid ChatBotId { get; } = Guid.NewGuid();
        public static Guid ArmatureId { get; } = Guid.NewGuid();

        private static readonly List<IdeaMarket> _ideaMarkets = CreateIdeaMarkets();

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
                    Status = IdeaMarketStatusType.RECRUITMENT_IS_OPEN,
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
                    Status = IdeaMarketStatusType.RECRUITMENT_IS_OPEN,
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
                    Status = IdeaMarketStatusType.RECRUITMENT_IS_OPEN,
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
                    Status = IdeaMarketStatusType.RECRUITMENT_IS_OPEN,
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
                    Status = IdeaMarketStatusType.RECRUITMENT_IS_CLOSED,
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
                    Status = IdeaMarketStatusType.RECRUITMENT_IS_CLOSED,
                    Requests = 0,
                    AcceptedRequests = 0,
                    IsFavorite = false
                }
            ];
        }

        public static IdeaMarket? GetIdeaMarketById(Guid id) =>
            _ideaMarkets.FirstOrDefault(im => im.Id == id);
    }
}
