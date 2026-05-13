namespace HITSBlazor.Components.Modals.RightSideModals.IdeaModal
{
    public class RatingRequest
    {
        public Guid? Id { get; set; }
        public byte? MarketValue { get; set; }
        public byte? Originality { get; set; }
        public byte? TechnicalRealizability { get; set; }
        public byte? Suitability { get; set; }
        public byte? Budget { get; set; }
    }
}
