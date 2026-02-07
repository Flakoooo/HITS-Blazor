using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Models.Teams.Entities
{
    public class Team
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Closed { get; set; }
        public bool IsRefused { get; set; }
        public int MembersCount { get; set; }
        public TeamMember Owner { get; set; } = new();
        public TeamMember? Leader { get; set; }

        public List<TeamMember> Members { get; set; } = [];
        public List<Skill> Skills { get; set; } = [];
        public List<Skill> WantedSkills { get; set; } = [];
        public TeamTags Tags { get; set; } = new();
        public bool? StatusQuest { get; set; }

        public bool HasActiveProject { get; set; }
        public bool IsAcceptedToIdea { get; set; }
    }
}
