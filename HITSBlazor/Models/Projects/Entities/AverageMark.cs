using HITSBlazor.Models.Projects.Enums;

namespace HITSBlazor.Models.Projects.Entities
{
    public class AverageMark
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public ProjectMemberRole ProjectRole { get; set; }
        public double Mark { get; set; }
        public List<Task>? Tasks { get; set; }
    }
}
