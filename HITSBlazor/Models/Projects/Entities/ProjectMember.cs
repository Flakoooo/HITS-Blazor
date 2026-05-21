using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Utils.Models;

namespace HITSBlazor.Models.Projects.Entities
{
    public class ProjectMember : ViewModelBase
    {
        public Guid? TeamId { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public ProjectMemberRole ProjectRole { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly FinishDate { get; set; }


        public string FullName => $"{FirstName} {LastName}";

        public override string GetDisplayInfo() => FullName;
        public override object GetId() => UserId;
    }
}
