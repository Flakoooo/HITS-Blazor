using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Models.Projects.Requests
{
    public class UpdateSprintTaskInfoRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Tag>? Tags { get; set; }
        public string? TeamLeadComment { get; set; }
        public string? ExecutorComment { get; set; }
    }
}
