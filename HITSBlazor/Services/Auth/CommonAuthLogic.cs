using HITSBlazor.Pages.Login;

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
    }
}
