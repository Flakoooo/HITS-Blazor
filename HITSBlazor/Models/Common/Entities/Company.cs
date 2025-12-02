using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Models.Common.Entities
{
    public class Company
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public User Owner { get; set; } = new();
        public List<User> Users { get; set; } = new();
    }
}
