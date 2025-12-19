namespace HITSBlazor.Models.Common.Entities
{
    public class UsersSkills
    {
        public Guid IdUsers { get; set; }
        public List<Skill> Skills { get; set; } = [];
    }
}
