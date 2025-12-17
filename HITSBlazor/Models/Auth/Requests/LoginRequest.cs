using System.ComponentModel.DataAnnotations;

namespace HITSBlazor.Models.Auth.Requests
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
