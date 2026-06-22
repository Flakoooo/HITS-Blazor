namespace HITSBlazor.Models.Users.Responses
{
    public class InvitationResponse
    {
        public string Email { get; set; } = string.Empty;
        public Guid Code { get; set; }
    }
}
