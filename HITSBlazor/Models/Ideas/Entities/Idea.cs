using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Models.Ideas.Entities
{
    public class Idea
    {
        public string Id { get; set; } = string.Empty;
        public User Initiator { get; set; } = new();
        public string CreatedAt { get; set; } = string.Empty;
        public string ModifiedAt { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Problem { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public IdeaStatusType Status { get; set; }
        public int MaxTeamSize { get; set; }
        public int MinTeamSize { get; set; }

        public UsersGroup? ProjectOffice { get; set; }
        public UsersGroup? Experts { get; set; }
        public string Customer { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;

        public int Suitability { get; set; }
        public int Budget { get; set; }
        public int PreAssessment { get; set; }
        public int? Rating { get; set; }

        public bool IsChecked { get; set; }
        public bool IsActive { get; set; }
    }
}
