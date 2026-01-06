using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Pages.Ideas.IdeasCreate
{
    public class IdeasCreateModel
    {
        //Может просто Id пользователя?
        //public Guid InitiatorId { get; set; }
        public User Initiator { get; set; } = new();

        public string Name { get; set; } = string.Empty;
        public string Problem { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;

        //Статус вообще нужно ли отправлять?
        //New и OnApproval ставятся на бэке
        public IdeaStatusType Status { get; set; }

        public int MaxTeamSize { get; set; }
        public int MinTeamSize { get; set; }

        //Может просто Id компании?
        //public Guid CompanyId { get; set; }
        public Company Customer { get; set; } = new();

        //Может просто Id пользователя?
        //public Guid ContactPersonId { get; set; }
        public User ContactPerson { get; set; } = new();

        public int Suitability { get; set; }
        public int Budget { get; set; }
    }
}
