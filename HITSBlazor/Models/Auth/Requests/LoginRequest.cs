using System.ComponentModel.DataAnnotations;

namespace HITSBlazor.Models.Auth.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Введите логин или email")]
        [Display(Name = "Логин")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;
    }
}
