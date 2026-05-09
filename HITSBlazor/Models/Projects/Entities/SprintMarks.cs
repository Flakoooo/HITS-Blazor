using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Utils.Models;

namespace HITSBlazor.Models.Projects.Entities
{
    public class SprintMarks : ViewModelBase
    {
        public Guid SprintId { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public ProjectMemberRole ProjectRole { get; set; }
        public int? Mark { get; set; }
        public int? CountCompletedTasks { get; set; }


        public string FullName => $"{FirstName} {LastName}";

        public override string GetDisplayInfo() => FullName;
        public override object GetId() => UserId;
    }
}
