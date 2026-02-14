using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Models.Common.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid IdeaId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Text { get; set; } = string.Empty;
        public User Sender { get; set; } = new();
        public List<Guid> CheckedBy { get; set; } = [];
    }
}
