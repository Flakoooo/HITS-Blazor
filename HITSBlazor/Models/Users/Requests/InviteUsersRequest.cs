using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Models.Users.Requests
{
    public class InviteUsersRequest
    {
        public List<string> Emails { get; set; } = [];
        public List<RoleType> Roles { get; set; } = [];
    }
}
