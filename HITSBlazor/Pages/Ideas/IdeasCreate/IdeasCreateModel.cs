using HITSBlazor.Models.Ideas.Enums;

namespace HITSBlazor.Pages.Ideas.IdeasCreate
{
    public class IdeasCreateModel
    {
        public string Name { get; set; } = string.Empty;
        public string Problem { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public IdeaStatusType Status { get; set; }

        public byte? MaxTeamSize { get; set; }
        public byte? MinTeamSize { get; set; }

        public string Customer { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;

        public byte Suitability { get; set; }
        public byte Budget { get; set; }
    }
}
