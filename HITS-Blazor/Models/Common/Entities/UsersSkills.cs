namespace HITSBlazor.Models.Common.Entities
{
    public class UsersSkills
    {
        public string IdUsers { get; set; } = string.Empty;
        public List<Skill> Skills { get; set; } = new();
    }
}
