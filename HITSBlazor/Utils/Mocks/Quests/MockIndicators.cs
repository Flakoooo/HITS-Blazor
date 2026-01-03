using HITSBlazor.Models.Quests.Entities;
using HITSBlazor.Models.Quests.Enums;

namespace HITSBlazor.Utils.Mocks.Quests
{
    public static class MockIndicators
    {
        private static readonly List<Indicator> _indicators = CreateIndicators();

        private static List<Indicator> CreateIndicators()
        {
            var softCategory = MockIndicatorCategories.GetIndicatorCategoryById(MockIndicatorCategories.SoftId)!;

            var answers = new List<string>() { "Хорошо", "Не очень" };

            return
            [
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Как вам отношения в команде?",
                    Answers = answers,
                    Type = IndicatorType.TEAM,
                    Role = IndicatorRoleType.Member,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Как вам профессионализм команды?",
                    Answers = answers,
                    Type = IndicatorType.TEAM,
                    Role = IndicatorRoleType.TeamLeader,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Каков ваш взгляд на коммуникацию с этим членом команды?",
                    Answers = answers,
                    Type = IndicatorType.MEMBER,
                    Role = IndicatorRoleType.Member,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Как оцениваете общение в команде?",
                    Answers = answers,
                    Type = IndicatorType.TEAM,
                    Role = IndicatorRoleType.TeamLeader,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Какие ваши мысли о навыках команды?",
                    Answers = answers,
                    Type = IndicatorType.TEAM,
                    Role = IndicatorRoleType.Member,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Как оцените атмосферу в коллективе?",
                    Answers = answers,
                    Type = IndicatorType.TEAM,
                    Role = IndicatorRoleType.Initiator,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Что вы думаете о взаимодействии с этим коллегой?",
                    Answers = answers,
                    Type = IndicatorType.MEMBER,
                    Role = IndicatorRoleType.Teacher,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Ваше мнение о компетентности команды?",
                    Answers = answers,
                    Type = IndicatorType.TEAM,
                    Role = IndicatorRoleType.Member,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Что вы думаете о взаимодействии внутри группы?",
                    Answers = answers,
                    Type = IndicatorType.MEMBER,
                    Role = IndicatorRoleType.Member,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Как воспринимаете атмосферу в команде?",
                    Answers = answers,
                    Type = IndicatorType.TEAM,
                    Role = IndicatorRoleType.Member,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Как вы оцениваете профессионализм коллег?",
                    Answers = answers,
                    Type = IndicatorType.TEAM,
                    Role = IndicatorRoleType.Member,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Ваш взгляд на взаимодействие с коллегами?",
                    Answers = answers,
                    Type = IndicatorType.MEMBER,
                    Role = IndicatorRoleType.Member,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Что вы думаете о внутренних отношениях в команде?",
                    Answers = answers,
                    Type = IndicatorType.TEAM,
                    Role = IndicatorRoleType.Member,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Как вы оцениваете профессиональные навыки команды?",
                    Answers = answers,
                    Type = IndicatorType.TEAM,
                    Role = IndicatorRoleType.Member,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Ваше мнение о взаимодействии с коллегами?",
                    Answers = answers,
                    Type = IndicatorType.MEMBER,
                    Role = IndicatorRoleType.Member,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Что вы думаете о взаимодействии в команде?",
                    Answers = answers,
                    Type = IndicatorType.MEMBER,
                    Role = IndicatorRoleType.Member,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Как вы оцениваете атмосферу в команде?",
                    Answers = answers,
                    Type = IndicatorType.TEAM,
                    Role = IndicatorRoleType.Member,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Что вы думаете о профессионализме команды?",
                    Answers = answers,
                    Type = IndicatorType.TEAM,
                    Role = IndicatorRoleType.Member,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                },
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    Name = "Ваш взгляд на взаимодействие с коллегой?",
                    Answers = answers,
                    Type = IndicatorType.MEMBER,
                    Role = IndicatorRoleType.Member,
                    IdCategory = softCategory.IdCategory,
                    CategoryName = softCategory.Name,
                    Visible = true
                }
            ];
        }

        public static List<Indicator> GetAllIndicators() => _indicators;
    }
}
