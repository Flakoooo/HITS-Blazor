using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Teams.Entities;

namespace HITSBlazor.Models.Users.Entities
{
    public class Profile : User
    {
        public List<Skill> Skills { get; set; } = [];
        public List<Idea> Ideas { get; set; } = [];
        public List<TeamExperience> Teams { get; set; } = [];
        public string? UserTag { get; set; }
        public bool? IsUserTagVisible { get; set; }
    }
}
