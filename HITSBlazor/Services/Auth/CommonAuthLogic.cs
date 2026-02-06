using HITSBlazor.Pages.Login;
using HITSBlazor.Pages.NewPassword;
using HITSBlazor.Pages.RecoveryPassword;
using HITSBlazor.Pages.Register;
using System.Text.RegularExpressions;

namespace HITSBlazor.Services.Auth
{
    public class CommonAuthLogic(GlobalNotificationService globalNotificationService)
    {
        public bool ValidateLoginModel(LoginModel loginModel)
        {
            bool emailIsEmpty = string.IsNullOrWhiteSpace(loginModel.Email);
            bool passwordIsEmpty = string.IsNullOrWhiteSpace(loginModel.Password);

            if (emailIsEmpty || passwordIsEmpty)
            {
                var errorMessage = "";

                if (emailIsEmpty && passwordIsEmpty)
                    errorMessage = "Пожалуйста, заполните логин и пароль";
                else if (emailIsEmpty)
                    errorMessage = "Пожалуйста, заполните логин";
                else
                    errorMessage = "Пожалуйста, заполните пароль";

                globalNotificationService.ShowError(errorMessage);
                return false;
            }

            var email = loginModel.Email.Trim();
            var atIndex = email.IndexOf('@');

            if (atIndex <= 0 || atIndex == email.Length - 1 || email.Count(c => c == '@') != 1)
            {
                globalNotificationService.ShowError("Неверный формат почты");
                return false;
            }

            if (loginModel.Password.Length < 8)
            {
                globalNotificationService.ShowError("Длина пароля не может быть меньше 8 символов");
                return false;
            }

            return true;
        }

        public bool ValidateNewPasswordModel(NewPasswordModel newPasswordModel)
        {
            bool codeIsEmpty = string.IsNullOrWhiteSpace(newPasswordModel.Code);
            bool passwordIsEmpty = string.IsNullOrWhiteSpace(newPasswordModel.Password);

            if (codeIsEmpty || passwordIsEmpty)
            {
                string? errorMessage;

                if (codeIsEmpty && passwordIsEmpty)
                    errorMessage = "Пожалуйста, введите код и новый пароль";
                else if (codeIsEmpty)
                    errorMessage = "Пожалуйста, введите код";
                else
                    errorMessage = "Пожалуйста, введите новый пароль";

                globalNotificationService.ShowError(errorMessage);
                return false;
            }

            if (newPasswordModel.Password.Length < 8)
            {
                globalNotificationService.ShowError("Длина пароля не может быть меньше 8 символов");
                return false;
            }

            return true;
        }

        public bool ValidateRecoveryModel(RecoveryModel recoveryModel)
        {
            if (string.IsNullOrWhiteSpace(recoveryModel.Email))
            {
                globalNotificationService.ShowError("Пожалуйста, укажите почту");
                return false;
            }

            var email = recoveryModel.Email.Trim();
            var atIndex = email.IndexOf('@');

            if (atIndex <= 0 || atIndex == email.Length - 1 || email.Count(c => c == '@') != 1)
            {
                globalNotificationService.ShowError("Неверный формат почты");
                return false;
            }

            return true;
        }

        public bool ValidateRegisterModel(RegisterModel registerModel)
        {
            string telephonePattern = @"^\+7\s\(\d{3}\)\s\d{3}-\d{2}-\d{2}$";
            string namePattern = @"^[А-ЯЁA-Z][а-яёa-z]+(-[А-ЯЁA-Z][а-яёa-z]+)?$";

            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(registerModel.Email))
                validationErrors.Add("почту");

            if (string.IsNullOrWhiteSpace(registerModel.FirstName))
                validationErrors.Add("имя");

            if (string.IsNullOrWhiteSpace(registerModel.LastName))
                validationErrors.Add("фамилию");

            if (string.IsNullOrWhiteSpace(registerModel.Password))
                validationErrors.Add("пароль");

            if (validationErrors.Count > 0)
            {
                var errorMessage = string.Empty;
                if (validationErrors.Count > 2)
                    errorMessage = "Заполните все поля";
                else
                    errorMessage = $"Заполните {validationErrors[0]}" + (validationErrors.Count == 2 ? $" и {validationErrors[1]}" : string.Empty);

                globalNotificationService.ShowError(errorMessage);
                return false;
            }

            var email = registerModel.Email.Trim();
            var atIndex = email.IndexOf('@');

            if (atIndex <= 0 || atIndex == email.Length - 1 || email.Count(c => c == '@') != 1)
            {
                globalNotificationService.ShowError("Неверный формат почты");
                return false;
            }

            if (!Regex.IsMatch(registerModel.FirstName, namePattern))
            {
                globalNotificationService.ShowError("Имя должно начинаться с заглавной буквы и содержать только буквы (и дефис для двойных имён)");
                return false;
            }

            if (!Regex.IsMatch(registerModel.LastName, namePattern))
            {
                globalNotificationService.ShowError("Фамилия должна начинаться с заглавной буквы и содержать только буквы (и дефис для двойных фамилий)");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(registerModel.Telephone) && !Regex.IsMatch(registerModel.Telephone, telephonePattern))
            {
                globalNotificationService.ShowError("Неверный формат телефона. Введите номер в формате: +7 (XXX) XXX-XX-XX");
                return false;
            }

            if (registerModel.Password.Length < 8)
            {
                globalNotificationService.ShowError("Длина пароля не может быть меньше 8 символов");
                return false;
            }

            return true;
        }
    }
}
