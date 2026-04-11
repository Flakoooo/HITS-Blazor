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

        public int? MaxTeamSize { get; set; }
        public int? MinTeamSize { get; set; }

        public string Customer { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;

        public int Suitability { get; set; }
        public int Budget { get; set; }
    }
}
