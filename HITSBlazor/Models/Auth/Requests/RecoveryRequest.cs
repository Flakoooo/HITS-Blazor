using System.ComponentModel.DataAnnotations;

namespace HITSBlazor.Models.Auth.Requests
{
    public class RecoveryRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
