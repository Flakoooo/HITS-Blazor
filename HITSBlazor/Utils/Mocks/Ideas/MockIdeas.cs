using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Pages.Ideas.IdeasCreate;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Users;
using Newtonsoft.Json.Linq;

namespace HITSBlazor.Utils.Mocks.Ideas
{
    public static class MockIdeas
    {
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
        public static Guid TestIdeaId { get; } = Guid.NewGuid();
        public static Guid ChatBotId { get; } = Guid.NewGuid();
        public static Guid ArmatureId { get; } = Guid.NewGuid();

        private static readonly Random _random = new();
        private static readonly List<Idea> _ideas = CreateIdeas();

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
            string hitsContactPersonFullName = hits.Users.FirstOrDefault()?.FullName ?? "null null";

            var projectOfficeGroup = MockUsersGroups.GetGroupById(MockUsersGroups.ProjectOfficeId)!;
            var expertGtoup = MockUsersGroups.GetGroupById(MockUsersGroups.ExpertsId)!;

            var ideas = new List<Idea>()
            {
                new Idea
                {
                    Id = RefactorId,
                    Initiator = kirill,
                    Name = "Рефактор кода",
                    Problem = _lorem,
                    Solution = _lorem,
                    Result = _lorem,
                    Description = _lorem,
                    CreatedAt = new DateTime(2023, 10, 21, 11, 2, 17, DateTimeKind.Utc),
                    ModifiedAt = new DateTime(2023, 10, 26, 11, 2, 17, DateTimeKind.Utc),
                    Status = IdeaStatusType.OnConfirmation,
                    MaxTeamSize = 7,
                    MinTeamSize = 3,
                    Customer = hits.Name,
                    ContactPerson = hitsContactPersonFullName,
                    IsChecked = true
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
                    CreatedAt = new DateTime(2023, 10, 24, 11, 2, 17, DateTimeKind.Utc),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    Status = IdeaStatusType.OnApproval,
                    MaxTeamSize = 4,
                    MinTeamSize = 3,
                    Customer = hits.Name,
                    ContactPerson = hitsContactPersonFullName,
                    IsChecked = false
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
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    Status = IdeaStatusType.New,
                    MaxTeamSize = 5,
                    MinTeamSize = 5,
                    Customer = hits.Name,
                    ContactPerson = hitsContactPersonFullName,
                    IsChecked = false
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
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    Status = IdeaStatusType.Confirmed,
                    MaxTeamSize = 5,
                    MinTeamSize = 5,
                    Customer = hits.Name,
                    ContactPerson = hitsContactPersonFullName,
                    IsChecked = false
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
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    Status = IdeaStatusType.Confirmed,
                    MaxTeamSize = 5,
                    MinTeamSize = 5,
                    Customer = hits.Name,
                    ContactPerson = hitsContactPersonFullName,
                    IsChecked = true
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
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    Status = IdeaStatusType.OnMarket,
                    MaxTeamSize = 7,
                    MinTeamSize = 5,
                    Customer = hits.Name,
                    ContactPerson = hitsContactPersonFullName,
                    IsChecked = false
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
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    Status = IdeaStatusType.OnMarket,
                    MaxTeamSize = 7,
                    MinTeamSize = 5,
                    Customer = "null",
                    ContactPerson = $"{anton.FirstName} {anton.LastName}",
                    IsChecked = false
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
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    Status = IdeaStatusType.OnMarket,
                    MaxTeamSize = 5,
                    MinTeamSize = 5,
                    Customer = "null",
                    ContactPerson = $"{lubov.FirstName} {lubov.LastName}",
                    IsChecked = false
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
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    Status = IdeaStatusType.OnMarket,
                    MaxTeamSize = 7,
                    MinTeamSize = 5,
                    Customer = "null",
                    ContactPerson = $"{dmitry.FirstName} {dmitry.LastName}",
                    IsChecked = false
                },
                new Idea
                {
                    Id = TestIdeaId,
                    Initiator = kirill,
                    Name = "Идея для проверки",
                    Problem = "null",
                    Solution = "Разработать web приложение для прогнозирования предпочтительного направления обучения поступающих , прогноза успеваемости и успешности с применением технологий машинного обучения и искусственного интеллекта",
                    Result = "null",
                    Description = "Разработать web приложение для прогнозирования предпочтительного направления обучения поступающих , прогноза успеваемости и успешности с применением технологий машинного обучения и искусственного интеллекта",
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    Status = IdeaStatusType.OnMarket,
                    MaxTeamSize = 7,
                    MinTeamSize = 5,
                    Customer = hits.Name,
                    ContactPerson = hitsContactPersonFullName,
                    IsChecked = false
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
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    Status = IdeaStatusType.OnMarket,
                    MaxTeamSize = 7,
                    MinTeamSize = 5,
                    Customer = hits.Name,
                    ContactPerson = hitsContactPersonFullName,
                    IsChecked = false
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
                    CreatedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    ModifiedAt = new DateTime(2023, 10, 25, 11, 2, 17, DateTimeKind.Utc),
                    Status = IdeaStatusType.OnMarket,
                    MaxTeamSize = 7,
                    MinTeamSize = 5,
                    Customer = hits.Name,
                    ContactPerson = hitsContactPersonFullName,
                    IsChecked = false
                }
            };

            foreach (var idea in ideas)
            {
                idea.Experts = expertGtoup.WithUsers(
                        MockUsersGroups.GetRandomGroupUsersById(MockUsersGroups.ExpertsId, _random.Next(1, 3))
                );
                idea.ProjectOffice = projectOfficeGroup.WithUsers(
                    MockUsersGroups.GetRandomGroupUsersById(MockUsersGroups.ProjectOfficeId, 1)
                );

                idea.Budget = _random.Next(1, 6);
                idea.Suitability = _random.Next(1, 6);

                idea.PreAssessment = Formulas.CalculcateRating(
                    [idea.Budget, idea.Suitability]
                );

                idea.Rating = MockRatings.CreateRatingByIdea(idea);
                idea.IsActive = true;
            }

            return ideas;
        }

        public static List<Idea> GetAllIdeas() => [.. _ideas];

        public static Idea? GetIdeaById(Guid id)
            => _ideas.FirstOrDefault(i => i.Id == id);

        public static List<Idea> GetIdeasByInitiatorId(Guid initiatorId)
            => [.. _ideas.Where(i => i.Initiator.Id == initiatorId)];

        public static bool CreateNewIdea(IdeasCreateModel model, User initiator)
        {
            var createdDate = DateTime.UtcNow;

            var idea = new Idea()
            {
                Id = Guid.NewGuid(),
                Initiator = initiator,
                CreatedAt = createdDate,
                ModifiedAt = createdDate,

                Name = model.Name,
                Problem = model.Problem,
                Description = model.Description,
                Solution = model.Solution,
                Result = model.Result,
                Status = model.Status,
                MaxTeamSize = model.MaxTeamSize,
                MinTeamSize = model.MinTeamSize,

                ProjectOffice = MockUsersGroups.GetGroupById(MockUsersGroups.ProjectOfficeId)!.WithUsers(
                    MockUsersGroups.GetRandomGroupUsersById(MockUsersGroups.ProjectOfficeId, 1)
                ),
                Experts = MockUsersGroups.GetGroupById(MockUsersGroups.ExpertsId)!.WithUsers(
                    MockUsersGroups.GetRandomGroupUsersById(MockUsersGroups.ExpertsId, _random.Next(1, 3))
                ),
                Customer = model.Customer,
                ContactPerson = model.ContactPerson,

                Suitability = model.Suitability,
                Budget = model.Budget,
                PreAssessment = Formulas.CalculcateRating([model.Suitability, model.Budget]),
                Rating = null,

                IsChecked = false,
                IsActive = true
            };
            _ideas.Add(idea);

            return true;
        }

        public static bool CheckIdea(Guid ideaId)
        {
            var idea = _ideas.FirstOrDefault(i => i.Id == ideaId);
            if (idea is null) return false;

            idea.IsChecked = true;
            return true;
        }

        public static bool UpdateIdea(Guid ideaId, IdeasCreateModel model)
        {
            var idea = _ideas.FirstOrDefault(i => i.Id == ideaId);
            if (idea is null) return false;

            idea.Name = model.Name;
            idea.Problem = model.Problem;
            idea.Description = model.Description;
            idea.Solution = model.Solution;
            idea.Result = model.Result;

            idea.MaxTeamSize = model.MaxTeamSize;
            idea.MinTeamSize = model.MinTeamSize;

            idea.Customer = model.Customer;
            idea.ContactPerson = model.ContactPerson;

            idea.Suitability = model.Suitability;
            idea.Budget = model.Budget;
            idea.PreAssessment = Formulas.CalculcateRating([model.Suitability, model.Budget]);

            return true;
        }

        public static Idea? UpdateIdeaStatus(Guid ideaId, IdeaStatusType ideaStatus)
        {
            var idea = _ideas.FirstOrDefault(i => i.Id == ideaId);
            if (idea is null) return null;

            idea.Status = ideaStatus;
            idea.ModifiedAt = DateTime.UtcNow;
            return idea;
        }

        public static bool DeleteIdea(Idea idea) => _ideas.Remove(idea);
    }
}
