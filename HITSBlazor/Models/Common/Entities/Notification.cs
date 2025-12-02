namespace HITSBlazor.Models.Common.Entities
{
    public class Notification
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Link { get; set; }
        public bool IsShowed { get; set; }
        public bool IsReaded { get; set; }
        public bool IsFavourite { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }
}
