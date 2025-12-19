namespace HITSBlazor.Models.Common.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool Confirmed { get; set; }
        public Guid CreatorId { get; set; } //nullable
        public Guid UpdaterId { get; set; } //nullable
        public Guid DeleterId { get; set; } //nullable
    }
}
