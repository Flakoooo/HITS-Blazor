namespace HITSBlazor.Models.Common.Entities
{
    public class Tag
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool Confirmed { get; set; }
        public string? CreatorId { get; set; }
        public string? UpdaterId { get; set; }
        public string? DeleterId { get; set; }
    }
}
