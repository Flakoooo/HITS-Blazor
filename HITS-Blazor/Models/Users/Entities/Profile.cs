using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Teams.Entities;

namespace HITSBlazor.Models.Users.Entities
{
    public class Profile : User
    {
        public List<Skill> Skills { get; set; } = new();
        public List<Idea> Ideas { get; set; } = new();
        public List<TeamExperience> Teams { get; set; } = new();
        public string? UserTag { get; set; }
        public bool? IsUserTagVisible { get; set; }
    }
}
