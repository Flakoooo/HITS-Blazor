namespace HITSBlazor.Models.Users.Entities
{
    public class UserTelegram
    {
        public Guid UserId { get; set; }
        public string UserTag { get; set; } = string.Empty;
        public Guid ChatId { get; set; } //nullable
        public bool IsVisible { get; set; }
    }
}
