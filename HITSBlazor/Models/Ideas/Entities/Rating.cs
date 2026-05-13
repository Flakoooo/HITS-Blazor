namespace HITSBlazor.Models.Ideas.Entities
{
    public class Rating
    {
        public Guid Id { get; set; }
        public Guid IdeaId { get; set; }
        public Guid ExpertId { get; set; }
        public string ExpertFirstName { get; set; } = string.Empty;
        public string ExpertLastName { get; set; } = string.Empty;

        public byte? MarketValue { get; set; }
        public byte? Originality { get; set; }
        public byte? TechnicalRealizability { get; set; }
        public byte? Suitability { get; set; }
        public byte? Budget { get; set; }
        public double? RatingValue { get; set; }
        public bool IsConfirmed { get; set; }

        public string ExpertFullName => $"{ExpertFirstName} {ExpertLastName}";
    }
}
