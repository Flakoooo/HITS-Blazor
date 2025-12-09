namespace HITSBlazor.Models.Ideas.Entities
{
    public class Rating
    {
        public string Id { get; set; } = string.Empty;
        public string IdeaId { get; set; } = string.Empty;
        public string ExpertId { get; set; } = string.Empty;
        public string ExpertFirstName { get; set; } = string.Empty;
        public string ExpertLastName { get; set; } = string.Empty;

        public int? MarketValue { get; set; }
        public int? Originality { get; set; }
        public int? TechnicalRealizability { get; set; }
        public int? Suitability { get; set; }
        public int? Budget { get; set; }
        public double? RatingValue { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
