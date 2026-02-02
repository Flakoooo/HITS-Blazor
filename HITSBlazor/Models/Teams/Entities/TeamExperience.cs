namespace HITSBlazor.Models.Teams.Entities
{
    public class TeamExperience
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public bool HasActiveProject { get; set; }
    }
}
