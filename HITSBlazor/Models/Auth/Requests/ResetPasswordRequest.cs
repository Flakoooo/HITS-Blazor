using System.ComponentModel.DataAnnotations;

namespace HITSBlazor.Models.Auth.Requests
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Код обязателен")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Код должен состоять из 6 символов")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Код должен содержать только цифры")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(8, ErrorMessage = "Пароль должен состоять как минимум из 8 символов")]
        public string Password { get; set; } = string.Empty;
    }
}
