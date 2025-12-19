using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Models.Common.Entities
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public User Owner { get; set; } = new();
        public List<User> Users { get; set; } = [];
    }
}
