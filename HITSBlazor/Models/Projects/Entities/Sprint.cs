using HITSBlazor.Models.Projects.Enums;

namespace HITSBlazor.Models.Projects.Entities
{
    public class Sprint
    {
        public string Id { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Goal { get; set; }
        public string Report { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string FinishDate { get; set; } = string.Empty;
        public int WorkingHours { get; set; }
        public SprintStatus Status { get; set; }
        public List<Task> Tasks { get; set; } = new();
    }
}
