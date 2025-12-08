using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Utils.Mocks.Common;
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

        public static List<IdeaMarket> GetMockIdeaMarkets() => [
            new IdeaMarket
            {
                Id = HelperId,
                IdeaId = "dfaedabe-2b3b-44e7-851e-35c9c6409869",
                Initiator = MockUsers.GetUserById(MockUsers.KirillId),
                Team = null,
                MarketId = MockMarkets.Autumn2023Id,
                Name = "Цифровой помощник сотрудника приемной комиссии университета",
                Problem = "null",
                Description = "null",
                Solution = "Разработать web приложение для прогнозирования предпочтительного направления обучения поступающих , прогноза успеваемости и успешности с применением технологий машинного обучения и искусственного интеллекта",
                Result = "null",
                MaxTeamSize = 7,
                Customer = "null",
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
                IdeaId = "279b3fbf-de63-426f-bf9f-0c01ea19706b",
                Initiator = MockUsers.GetUserById(MockUsers.AntonId),
                Team = null,
                MarketId = MockMarkets.Autumn2023Id,
                Name = "PW Technology: Интерактивное виртуальное образование с 3D тренажерами",
                Problem = "null",
                Description = "null",
                Solution = "Разработка проекта по обучению людей взаимодействию с оборудованием в игровой форме, где каждый пользователь сможет примерить на себя роль нефтяника, электрика, строителя и других профессий, требующих специальных навыков и знаний.",
                Result = "null",
                MaxTeamSize = 7,
                Customer = "null",
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
                IdeaId = "c2d8b869-2731-4c37-b0de-17c1950a770f",
                Initiator = MockUsers.GetUserById(MockUsers.LubovId),
                Team = null,
                MarketId = MockMarkets.Autumn2023Id,
                Name = "EMetrics.Viewer",
                Problem = "null",
                Description = "null",
                Solution = "WEB-приложение (конкретная реализация выбирается разработчиками в рамках стэка React|Vue, NodeJS|Python)\nИспользование существующих у заказчика оперативных и аналитических БД в том числе тестовых.\nИспользование VPN (предоставляется заказчиком) для подключения к ресурсам заказчика с машин разработчиков.",
                Result = "null",
                MaxTeamSize = 5,
                Customer = "null",
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
                IdeaId = "a51c17fd-c089-448c-be7b-714bceee8109",
                Initiator = MockUsers.GetUserById(MockUsers.DmitryId),
                Team = null,
                MarketId = MockMarkets.Autumn2023Id,
                Name = "Расчет строительных материалов.",
                Problem = "null",
                Description = "null",
                Solution = "Система, которая позволяет указать размерные характеристики объекта строительства и\\или работ, выбрать требуемый материал и получить расчет количества этих материалов.\nПродумать систему придется в рамках реализации проекта.",
                Result = "null",
                MaxTeamSize = 7,
                Customer = "null",
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
                IdeaId = "a51c17fd-c089-448c-be7b-714bceee8109",
                Initiator = MockUsers.GetUserById(MockUsers.KirillId),
                Team = null,
                MarketId = MockMarkets.Autumn2023Id,
                Name = "Идея для проверки",
                Problem = "null",
                Description = "Разработать web приложение для прогнозирования предпочтительного направления обучения поступающих , прогноза успеваемости и успешности с применением технологий машинного обучения и искусственного интеллекта",
                Solution = "Разработать web приложение для прогнозирования предпочтительного направления обучения поступающих , прогноза успеваемости и успешности с применением технологий машинного обучения и искусственного интеллекта",
                Result = "null",
                MaxTeamSize = 7,
                Customer = "ВШЦТ",
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
            }
        ];

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
