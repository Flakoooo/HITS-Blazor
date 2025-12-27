namespace HITSBlazor.Models.Common.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Link { get; set; }
        public bool IsShowed { get; set; }
        public bool IsReaded { get; set; }
        public bool IsFavorite { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }
}
