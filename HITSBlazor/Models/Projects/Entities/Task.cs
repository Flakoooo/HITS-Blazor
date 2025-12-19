using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Models.Projects.Entities
{
    public class Task
    {
        public Guid Id { get; set; }
        public Guid SprintId { get; set; } // nullable
        public Guid ProjectId { get; set; }
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

        public List<Tag> Tags { get; set; } = [];
        public Enums.TaskStatus Status { get; set; }
    }
}
