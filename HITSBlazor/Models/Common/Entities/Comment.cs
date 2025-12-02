namespace HITSBlazor.Models.Common.Entities
{
    public class Comment
    {
        public string Id { get; set; } = string.Empty;
        public string IdeaId { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public User Sender { get; set; } = new();
        public List<string> CheckedBy { get; set; } = new();
    }
}
