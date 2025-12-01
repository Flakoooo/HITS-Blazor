namespace Domain
{
    public enum IdeaStatusType
    {
        NEW,
        ON_EDITING,
        ON_APPROVAL,
        ON_CONFIRMATION,
        CONFIRMED,
        ON_MARKET
    }

    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        // Добавьте другие свойства User, если они нужны для Idea
    }

    public class UsersGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }
        // Добавьте другие свойства UsersGroup, если они нужны для Idea
    }

    public class Skill
    {
        public string Id { get; set; }
        public string Name { get; set; }
        // Добавьте другие свойства Skill, если они нужны
    }

    public class Idea
    {
        public string Id { get; set; }
        public User Initiator { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        public string Name { get; set; }
        public string Problem { get; set; }
        public string Description { get; set; }
        public string Solution { get; set; }
        public string Result { get; set; }
        public IdeaStatusType Status { get; set; }
        public int MaxTeamSize { get; set; }
        public int MinTeamSize { get; set; }

        public UsersGroup ProjectOffice { get; set; }
        public UsersGroup Experts { get; set; }
        public string Customer { get; set; }
        public string ContactPerson { get; set; }

        public int Suitability { get; set; }
        public int Budget { get; set; }
        public int PreAssessment { get; set; }
        public int? Rating { get; set; }

        public bool IsChecked { get; set; }
        public bool IsActive { get; set; }
    }

    public class Rating
    {
        public string Id { get; set; }
        public string IdeaId { get; set; }
        public string ExpertId { get; set; }
        public string ExpertFirstName { get; set; }
        public string ExpertLastName { get; set; }

        public int? MarketValue { get; set; }
        public int? Originality { get; set; }
        public int? TechnicalRealizability { get; set; }
        public int? Suitability { get; set; }
        public int? Budget { get; set; }
        public int? RatingValue { get; set; } // Переименовано, чтобы избежать конфликта с свойством Rating
        public bool IsConfirmed { get; set; }
    }

    public class IdeaSkills
    {
        public string IdeaId { get; set; }
        public List<Skill> Skills { get; set; }
    }
}