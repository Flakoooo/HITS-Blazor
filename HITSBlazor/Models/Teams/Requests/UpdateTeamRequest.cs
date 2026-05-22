namespace HITSBlazor.Models.Teams.Requests
{
    public class UpdateTeamRequest
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsClosed { get; set; }
        public List<Guid> WantedSkills { get; set; } = [];

        public Guid? NewOwnerId { get; set; }
        public Guid? NewLeaderId { get; set; }
        public List<Guid> InvitedMembers { get; set; } = [];
        public List<Guid> KickedMembers { get; set; } = [];
    }
}
