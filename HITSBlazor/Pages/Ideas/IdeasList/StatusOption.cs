using HITSBlazor.Models.Ideas.Enums;

namespace HITSBlazor.Pages.Ideas.IdeasList
{
    public class StatusOption
    {
        public IdeaStatusType Value { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
