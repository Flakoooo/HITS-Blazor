using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Models.Projects.Entities
{
    public class TaskMovementLog
    {
        public string Id { get; set; } = string.Empty;
        public Task Task { get; set; } = new();
        public User? Executor { get; set; }
        public User User { get; set; } = new();
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string WastedTime { get; set; } = string.Empty;
        public Enums.TaskStatus Status { get; set; }
    }
}
