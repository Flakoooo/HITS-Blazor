using System.Text.Json.Serialization;

namespace HITSBlazor.Pages.NewPassword
{
    public class NewPasswordModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
