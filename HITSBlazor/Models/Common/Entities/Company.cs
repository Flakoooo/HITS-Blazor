using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Models;

namespace HITSBlazor.Models.Common.Entities
{
    public class Company : ViewModelBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public User Owner { get; set; } = new();
        public List<User> Members { get; set; } = [];


        public override string GetDisplayInfo() => Name;
        public override object GetId() => Id;
    }
}
