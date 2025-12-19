using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Ideas
{
    public static class MockIdeas
    {
        private static readonly List<Idea> _ideas = CreateIdeas();

        private static readonly string _lorem = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Eius aperiam delectus possimus, voluptates quo accusamus? Consequatur, quasi rem temporibus blanditiis delectus aliquid officia aut, totam incidunt reiciendis eaque laborum fugiat!";

        public static Guid RefactorId { get; } = Guid.NewGuid();
        public static Guid MyNewIdeaId { get; } = Guid.NewGuid();
        public static Guid Idea2Id { get; } = Guid.NewGuid();
        public static Guid FoldingBedId { get; } = Guid.NewGuid();
        public static Guid Strawberry1Id { get; } = Guid.NewGuid();
        public static Guid HelperId { get; } = Guid.NewGuid();
        public static Guid PWTechnologyId { get; } = Guid.NewGuid();
        public static Guid EMetricsViewerId { get; } = Guid.NewGuid();
        public static Guid CalculatorId { get; } = Guid.NewGuid();
        public static Guid ChatBotId { get; } = Guid.NewGuid();
        public static Guid ArmatureId { get; } = Guid.NewGuid();

        private static List<Idea> CreateIdeas()
        {
            var kirill = MockUsers.GetUserById(MockUsers.KirillId)!;
            var ivan = MockUsers.GetUserById(MockUsers.IvanId)!;
            var manager = MockUsers.GetUserById(MockUsers.ManagerId)!;
            var anton = MockUsers.GetUserById(MockUsers.AntonId)!;
            var lubov = MockUsers.GetUserById(MockUsers.LubovId)!;
            var dmitry = MockUsers.GetUserById(MockUsers.DmitryId)!;
            var alex = MockUsers.GetUserById(MockUsers.AlexId)!;
            var admin = MockUsers.GetUserById(MockUsers.AdminId)!;

            var hits = MockCompanies.GetCompanyById(MockCompanies.HITSId)!;

            return
            [
                new Idea
                {
                    Id = RefactorId,
                    Initiator = kirill,
                    Name = "Рефактор кода",
                    Problem = _lorem,
                    Solution = _lorem,
                    Result = _lorem,
                    Description = _lorem,
                    CreatedAt = new DateTime(2023, 10, 21, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    ModifiedAt = new DateTime(2023, 10, 26, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Status = IdeaStatusType.ON_CONFIRMATION,
                    MaxTeamSize = 7,
                    MinTeamSize = 3,
                    Customer = hits.Name,
                    ContactPerson = hits.Name,
                    Experts = null,
                    ProjectOffice = null,
                    Budget = 1,
                    Suitability = 1,
                    PreAssessment = 1,
                    Rating = null,
                    IsChecked = true,
                    IsActive = false
                },
                new Idea
                {
                    Id = MyNewIdeaId,
                    Initiator = ivan,
                    Name = "Моя новая идея",
                    Problem = _lorem,
                    Solution = _lorem,
                    Result = _lorem,
                    Description = _lorem,
                    CreatedAt = new DateTime(2023, 10, 24, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Status = IdeaStatusType.ON_APPROVAL,
                    MaxTeamSize = 4,
                    MinTeamSize = 3,
                    Customer = hits.Name,
                    ContactPerson = hits.Name,
                    Experts = null,
                    ProjectOffice = null,
                    Budget = 4,
                    Suitability = 3,
                    PreAssessment = 4,
                    Rating = 2,
                    IsChecked = false,
                    IsActive = false
                },
                new Idea
                {
                    Id = Idea2Id,
                    Initiator = manager,
                    Name = "Идея 2",
                    Problem = _lorem,
                    Solution = _lorem,
                    Result = _lorem,
                    Description = _lorem,
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Status = IdeaStatusType.NEW,
                    MaxTeamSize = 5,
                    MinTeamSize = 5,
                    Customer = hits.Name,
                    ContactPerson = hits.Name,
                    Experts = null,
                    ProjectOffice = null,
                    Budget = 4,
                    Suitability = 3,
                    PreAssessment = 4,
                    Rating = 4,
                    IsChecked = false,
                    IsActive = false
                },
                new Idea
                {
                    Id = FoldingBedId,
                    Initiator = kirill,
                    Name = "Раскладушка",
                    Problem = _lorem,
                    Solution = _lorem,
                    Result = _lorem,
                    Description = _lorem,
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Status = IdeaStatusType.CONFIRMED,
                    MaxTeamSize = 5,
                    MinTeamSize = 5,
                    Customer = hits.Name,
                    ContactPerson = hits.Name,
                    Experts = null,
                    ProjectOffice = null,
                    Budget = 4,
                    Suitability = 3,
                    PreAssessment = 4,
                    Rating = 4,
                    IsChecked = false,
                    IsActive = false
                },
                new Idea
                {
                    Id = Strawberry1Id,
                    Initiator = ivan,
                    Name = "Земляника",
                    Problem = _lorem,
                    Solution = _lorem,
                    Result = _lorem,
                    Description = _lorem,
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Status = IdeaStatusType.CONFIRMED,
                    MaxTeamSize = 5,
                    MinTeamSize = 5,
                    Customer = hits.Name,
                    ContactPerson = hits.Name,
                    Experts = null,
                    ProjectOffice = null,
                    Budget = 4,
                    Suitability = 3,
                    PreAssessment = 4,
                    Rating = 4,
                    IsChecked = true,
                    IsActive = false
                },
                new Idea
                {
                    Id = HelperId,
                    Initiator = kirill,
                    Name = "Цифровой помощник сотрудника приемной комиссии университета",
                    Problem = "null",
                    Solution = "Разработать web приложение для прогнозирования предпочтительного направления обучения поступающих , прогноза успеваемости и успешности с применением технологий машинного обучения и искусственного интеллекта",
                    Result = "null",
                    Description = "null",
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Status = IdeaStatusType.ON_MARKET,
                    MaxTeamSize = 7,
                    MinTeamSize = 5,
                    Customer = hits.Name,
                    ContactPerson = hits.Name,
                    Experts = null,
                    ProjectOffice = null,
                    Budget = 4,
                    Suitability = 3,
                    PreAssessment = 4,
                    Rating = 4,
                    IsChecked = false,
                    IsActive = true
                },
                new Idea
                {
                    Id = PWTechnologyId,
                    Initiator = anton,
                    Name = "PW Technology: Интерактивное виртуальное образование с 3D тренажерами",
                    Problem = "null",
                    Solution = "Разработка проекта по обучению людей взаимодействию с оборудованием в игровой форме, где каждый пользователь сможет примерить на себя роль нефтяника, электрика, строителя и других профессий, требующих специальных навыков и знаний.",
                    Result = "null",
                    Description = "null",
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Status = IdeaStatusType.ON_MARKET,
                    MaxTeamSize = 7,
                    MinTeamSize = 5,
                    Customer = "null",
                    ContactPerson = $"{anton.FirstName} {anton.LastName}",
                    Experts = null,
                    ProjectOffice = null,
                    Budget = 4,
                    Suitability = 3,
                    PreAssessment = 4,
                    Rating = 4,
                    IsChecked = false,
                    IsActive = true
                },
                new Idea
                {
                    Id = EMetricsViewerId,
                    Initiator = lubov,
                    Name = "EMetrics.Viewer",
                    Problem = "null",
                    Solution = "WEB-приложение (конкретная реализация выбирается разработчиками в рамках стэка React|Vue, NodeJS|Python)\nИспользование существующих у заказчика оперативных и аналитических БД в том числе тестовых.\nИспользование VPN (предоставляется заказчиком) для подключения к ресурсам заказчика с машин разработчиков.",
                    Result = "null",
                    Description = "null",
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Status = IdeaStatusType.ON_MARKET,
                    MaxTeamSize = 5,
                    MinTeamSize = 5,
                    Customer = "null",
                    ContactPerson = $"{lubov.FirstName} {lubov.LastName}",
                    Experts = null,
                    ProjectOffice = null,
                    Budget = 4,
                    Suitability = 3,
                    PreAssessment = 4,
                    Rating = 4,
                    IsChecked = false,
                    IsActive = true
                },
                new Idea
                {
                    Id = CalculatorId,
                    Initiator = dmitry,
                    Name = "Расчет строительных материалов.",
                    Problem = "null",
                    Solution = "Система, которая позволяет указать размерные характеристики объекта строительства и\\или работ, выбрать требуемый материал и получить расчет количества этих материалов.\nПродумать систему придется в рамках реализации проекта.",
                    Result = "null",
                    Description = "null",
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Status = IdeaStatusType.ON_MARKET,
                    MaxTeamSize = 7,
                    MinTeamSize = 5,
                    Customer = "null",
                    ContactPerson = $"{dmitry.FirstName} {dmitry.LastName}",
                    Experts = null,
                    ProjectOffice = null,
                    Budget = 4,
                    Suitability = 3,
                    PreAssessment = 4,
                    Rating = 4,
                    IsChecked = false,
                    IsActive = true
                },
                new Idea
                {
                    Id = CalculatorId,
                    Initiator = kirill,
                    Name = "Идея для проверки",
                    Problem = "null",
                    Solution = "Разработать web приложение для прогнозирования предпочтительного направления обучения поступающих , прогноза успеваемости и успешности с применением технологий машинного обучения и искусственного интеллекта",
                    Result = "null",
                    Description = "Разработать web приложение для прогнозирования предпочтительного направления обучения поступающих , прогноза успеваемости и успешности с применением технологий машинного обучения и искусственного интеллекта",
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Status = IdeaStatusType.ON_MARKET,
                    MaxTeamSize = 7,
                    MinTeamSize = 5,
                    Customer = hits.Name,
                    ContactPerson = hits.Name,
                    Experts = null,
                    ProjectOffice = null,
                    Budget = 4,
                    Suitability = 3,
                    PreAssessment = 4,
                    Rating = 4,
                    IsChecked = false,
                    IsActive = true
                },
                new Idea
                {
                    Id = ChatBotId,
                    Initiator = alex,
                    Name = "Чат-бот в telegram для запросов и обращений к HR вне системы 1С",
                    Problem = _lorem,
                    Solution = _lorem,
                    Result = _lorem,
                    Description = _lorem,
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Status = IdeaStatusType.ON_MARKET,
                    MaxTeamSize = 7,
                    MinTeamSize = 5,
                    Customer = hits.Name,
                    ContactPerson = hits.Name,
                    Experts = null,
                    ProjectOffice = null,
                    Budget = 4,
                    Suitability = 3,
                    PreAssessment = 4,
                    Rating = 4,
                    IsChecked = false,
                    IsActive = true
                },
                new Idea
                {
                    Id = ArmatureId,
                    Initiator = admin,
                    Name = "Прогнозирование закупок арматуры на основе исторических данных и обогащением доп. критериями",
                    Problem = _lorem,
                    Solution = _lorem,
                    Result = _lorem,
                    Description = _lorem,
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Status = IdeaStatusType.ON_MARKET,
                    MaxTeamSize = 7,
                    MinTeamSize = 5,
                    Customer = hits.Name,
                    ContactPerson = hits.Name,
                    Experts = null,
                    ProjectOffice = null,
                    Budget = 4,
                    Suitability = 3,
                    PreAssessment = 4,
                    Rating = 4,
                    IsChecked = false,
                    IsActive = true
                }
            ];
        }

        public static Idea? GetIdeaById(Guid id)
            => _ideas.FirstOrDefault(i => i.Id == id);

        public static List<Idea> GetIdeasByInitiatorId(Guid initiatorId)
            => [.. _ideas.Where(i => i.Initiator.Id == initiatorId)];
    }
}
