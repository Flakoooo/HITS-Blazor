using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Pages.Teams.TeamsCreate
{
    public class TeamsCreateModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Closed { get; set; } = false;

        public User Owner { get; set; } = new();
        public User Leader { get; set; } = new();

        public HashSet<User> Members { get; set; } = [];
        public List<Skill> WantedSkills { get; set; } = [];
    }
}
