using System.ComponentModel.DataAnnotations;

namespace HITSBlazor.Models.Auth.Requests
{
    public class RecoveryRequest
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Введите корректный email адрес")]
        public string Email { get; set; } = string.Empty;
    }
}
