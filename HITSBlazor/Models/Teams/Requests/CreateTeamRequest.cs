namespace HITSBlazor.Models.Teams.Requests
{
    public class CreateTeamRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsClosed { get; set; } = false;
        public Guid OwnerId { get; set; }
        public Guid? LeaderId { get; set; }
        public List<Guid> Members { get; set; } = [];
        public List<Guid> WantedSkills { get; set; } = [];
    }
}
