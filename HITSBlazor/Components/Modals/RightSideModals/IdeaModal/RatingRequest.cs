namespace HITSBlazor.Components.Modals.RightSideModals.IdeaModal
{
    public class RatingRequest
    {
        public Guid? Id { get; set; }
        public int? MarketValue { get; set; }
        public int? Originality { get; set; }
        public int? TechnicalRealizability { get; set; }
        public int? Suitability { get; set; }
        public int? Budget { get; set; }
    }
}
