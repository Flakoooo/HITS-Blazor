namespace HITSBlazor.Models.Teams.Entities
{
    public class TeamExperience
    {
        public string TeamId { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string? FinishDate { get; set; }
        public bool HasActiveProject { get; set; }
    }
}
