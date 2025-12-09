using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Ideas;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Markets
{
    public static class MockIdeaMarkets
    {
        public static string HelperId { get; } = Guid.NewGuid().ToString();
        public static string PWTechnologyId { get; } = Guid.NewGuid().ToString();
        public static string EMetricsViewerId { get; } = Guid.NewGuid().ToString();
        public static string CalculatorId { get; } = Guid.NewGuid().ToString();
        public static string TestId { get; } = Guid.NewGuid().ToString();

        public static List<IdeaMarket> GetMockIdeaMarkets()
        {
            var helper = MockIdeas.GetIdeaById(MockIdeas.HelperId);
            var pwTechnology = MockIdeas.GetIdeaById(MockIdeas.PWTechnologyId);
            var eMetricsViewer = MockIdeas.GetIdeaById(MockIdeas.EMetricsViewerId);
            var calculator = MockIdeas.GetIdeaById(MockIdeas.CalculatorId);
            var test = MockIdeas.GetIdeaById(MockIdeas.TestId);

            return [
                new IdeaMarket
                {
                    Id = HelperId,
                    IdeaId = helper!.Id,
                    Initiator = MockUsers.GetUserById(MockUsers.KirillId),
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
                    Stack =
                    [
                        MockSkills.GetSkillById(MockSkills.PythonId),
                        MockSkills.GetSkillById(MockSkills.KerasId),
                        MockSkills.GetSkillById(MockSkills.ScikitLearnId),
                        MockSkills.GetSkillById(MockSkills.PostgreSQLId),
                        MockSkills.GetSkillById(MockSkills.DockerId)
                    ],
                    Status = IdeaMarketStatusType.RECRUITMENT_IS_OPEN,
                    Requests = 0,
                    AcceptedRequests = 0,
                    IsFavorite = false
                },
                new IdeaMarket
                {
                    Id = PWTechnologyId,
                    IdeaId = pwTechnology!.Id,
                    Initiator = MockUsers.GetUserById(MockUsers.AntonId),
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
                        MockSkills.GetSkillById(MockSkills.UnrealEngineId),
                        MockSkills.GetSkillById(MockSkills.GitId),
                        MockSkills.GetSkillById(MockSkills.CppId)
                    ],
                    Status = IdeaMarketStatusType.RECRUITMENT_IS_OPEN,
                    Requests = 0,
                    AcceptedRequests = 0,
                    IsFavorite = false
                },
                new IdeaMarket
                {
                    Id = EMetricsViewerId,
                    IdeaId = eMetricsViewer!.Id,
                    Initiator = MockUsers.GetUserById(MockUsers.LubovId),
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
                    Stack =
                    [
                        MockSkills.GetSkillById(MockSkills.PythonId),
                        MockSkills.GetSkillById(MockSkills.DockerId),
                        MockSkills.GetSkillById(MockSkills.GoId),
                        MockSkills.GetSkillById(MockSkills.CSharpId),
                        MockSkills.GetSkillById(MockSkills.VueId),
                        MockSkills.GetSkillById(MockSkills.GitId),
                        MockSkills.GetSkillById(MockSkills.RedisId),
                        MockSkills.GetSkillById(MockSkills.PostgreSQLId)
                    ],
                    Status = IdeaMarketStatusType.RECRUITMENT_IS_OPEN,
                    Requests = 0,
                    AcceptedRequests = 0,
                    IsFavorite = false
                },
                new IdeaMarket
                {
                    Id = CalculatorId,
                    IdeaId = calculator!.Id,
                    Initiator = MockUsers.GetUserById(MockUsers.DmitryId),
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
                    Stack =
                    [
                        MockSkills.GetSkillById(MockSkills.PythonId),
                        MockSkills.GetSkillById(MockSkills.DockerId),
                        MockSkills.GetSkillById(MockSkills.GoId),
                        MockSkills.GetSkillById(MockSkills.CSharpId),
                        MockSkills.GetSkillById(MockSkills.VueId),
                        MockSkills.GetSkillById(MockSkills.GitId),
                        MockSkills.GetSkillById(MockSkills.RedisId),
                        MockSkills.GetSkillById(MockSkills.PostgreSQLId)
                    ],
                    Status = IdeaMarketStatusType.RECRUITMENT_IS_OPEN,
                    Requests = 0,
                    AcceptedRequests = 0,
                    IsFavorite = false
                },
                new IdeaMarket
                {
                    Id = TestId,
                    IdeaId = test!.Id,
                    Initiator = MockUsers.GetUserById(MockUsers.KirillId),
                    Team = null,
                    MarketId = MockMarkets.Autumn2023Id,
                    Name = test.Name,
                    Problem = test.Problem,
                    Description = test.Description,
                    Solution = test.Solution,
                    Result = test.Result,
                    MaxTeamSize = test.MaxTeamSize,
                    Customer = test.Customer,
                    Position = 11,
                    Stack =
                    [
                        MockSkills.GetSkillById(MockSkills.PythonId),
                        MockSkills.GetSkillById(MockSkills.KerasId),
                        MockSkills.GetSkillById(MockSkills.ScikitLearnId),
                        MockSkills.GetSkillById(MockSkills.PostgreSQLId),
                        MockSkills.GetSkillById(MockSkills.DockerId)
                    ],
                    Status = IdeaMarketStatusType.RECRUITMENT_IS_OPEN,
                    Requests = 0,
                    AcceptedRequests = 0,
                    IsFavorite = false
                }
            ];
        }

        public static IdeaMarket? GetIdeaMarketById(string id)
            => GetMockIdeaMarkets().FirstOrDefault(im => im.Id == id);

        public static List<IdeaMarket> GetIdeaMarketsByMarketId(string marketId)
            => [.. GetMockIdeaMarkets().Where(im => im.MarketId == marketId)];

        public static List<IdeaMarket> GetOpenRecruitmentIdeaMarkets()
            => [.. GetMockIdeaMarkets().Where(im => im.Status == IdeaMarketStatusType.RECRUITMENT_IS_OPEN)];

        public static List<IdeaMarket> GetIdeaMarketsByInitiator(string initiatorId)
            => [.. GetMockIdeaMarkets().Where(im => im.Initiator.Id == initiatorId)];

        public static List<IdeaMarket> GetFavoriteIdeaMarkets()
            => [.. GetMockIdeaMarkets().Where(im => im.IsFavorite)];

        public static List<IdeaMarket> GetIdeaMarketsWithTeam()
            => [.. GetMockIdeaMarkets().Where(im => im.Team != null)];
    }
}
