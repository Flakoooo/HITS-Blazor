using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Models.Projects.Requests
{
    public class CreateTaskRequest
    {
        public Guid? SprintId { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public User Initiator { get; set; } = new();
        public int WorkHour { get; set; }
        public DateTime StartDate { get; set; }
        public ICollection<Tag> Tags { get; set; } = [];
        public Enums.TaskStatus Status { get; set; }
    }
}
