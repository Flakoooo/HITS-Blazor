using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Utils.Models;

namespace HITSBlazor.Models.Projects.Entities
{
    public class ProjectMember : ViewModelBase
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public ProjectMemberRole ProjectRole { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }


        public string FullName => $"{FirstName} {LastName}";

        public override string GetDisplayInfo() => FullName;
        public override object GetId() => UserId;
    }
}
