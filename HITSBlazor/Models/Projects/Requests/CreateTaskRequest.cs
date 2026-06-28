using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Models.Projects.Requests
{
    public class CreateTaskRequest
    {
        public Guid ProjectId { get; set; }
        public Guid? SprintId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int WorkHour { get; set; }
        public DateTime StartDate { get; set; }
        public ICollection<Tag> Tags { get; set; } = [];
    }
}
