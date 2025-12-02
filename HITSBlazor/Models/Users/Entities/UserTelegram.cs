namespace HITSBlazor.Models.Users.Entities
{
    public class UserTelegram
    {
        public string UserId { get; set; } = string.Empty;
        public string UserTag { get; set; } = string.Empty;
        public string? ChatId { get; set; }
        public bool IsVisible { get; set; }
    }
}
