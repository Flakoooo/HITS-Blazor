using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Models.Projects.Entities
{
    public class Task
    {
        public string Id { get; set; } = string.Empty;
        public string? SprintId { get; set; }
        public string ProjectId { get; set; } = string.Empty;
        public int? Position { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? LeaderComment { get; set; }
        public string? ExecutorComment { get; set; }

        public User Initiator { get; set; } = new();
        public User? Executor { get; set; }
        public int WorkHour { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string? FinishDate { get; set; }

        public List<Tag> Tags { get; set; } = new();
        public TaskStatus Status { get; set; }
    }
}
