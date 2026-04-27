using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Models.Projects.Requests
{
    public class UpdateTaskRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Tag>? Tags { get; set; }
        public int? WorkHour { get; set; }
    }
}
