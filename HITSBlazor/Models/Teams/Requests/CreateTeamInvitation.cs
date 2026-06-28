namespace HITSBlazor.Models.Teams.Requests
{
    public class CreateTeamInvitation
    {
        public required Guid UserId { get; set; }
        public required Guid TeamId { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
