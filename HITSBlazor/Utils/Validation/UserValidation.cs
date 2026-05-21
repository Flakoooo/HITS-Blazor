using System.Text.RegularExpressions;

namespace HITSBlazor.Utils.Validation
{
    public class UserValidation
    {
        private const string _namePattern = @"^[А-ЯЁA-Z][а-яёa-z]+(-[А-ЯЁA-Z][а-яёa-z]+)?$";
        private const string _telephonePattern = @"^\+7\s\(\d{3}\)\s\d{3}-\d{2}-\d{2}$";
        //TODO: добавить паттерн для учебных групп

        public static ValidationResult EmailValidation(string verifiableEmail)
        {
            if (string.IsNullOrWhiteSpace(verifiableEmail))
                return ValidationResult.Fail("Почта не может быть пустой");

            var email = verifiableEmail.Trim();
            var atIndex = email.IndexOf('@');
            if (atIndex <= 0 || atIndex == email.Length - 1 || email.Count(c => c == '@') != 1)
                return ValidationResult.Fail("Неверный формат почты");

            return ValidationResult.Ok();
        }

        public static ValidationResult FirstNameValidation(string verifiableFirstName)
        {
            if (string.IsNullOrWhiteSpace(verifiableFirstName))
                return ValidationResult.Fail("Имя не может быть пустым");

            if (!Regex.IsMatch(verifiableFirstName, _namePattern))
                return ValidationResult.Fail("Имя должно начинаться с заглавной буквы и содержать только буквы (и дефис для двойных имён)");

            return ValidationResult.Ok();
        }

        public static ValidationResult LastNameValidation(string verifiableLastName)
        {
            if (string.IsNullOrWhiteSpace(verifiableLastName))
                return ValidationResult.Fail("Фамилия не может быть пустой");

            if (!Regex.IsMatch(verifiableLastName, _namePattern))
                return ValidationResult.Fail("Фамилия должна начинаться с заглавной буквы и содержать только буквы (и дефис для двойных фамилий)");

            return ValidationResult.Ok();
        }

        public static ValidationResult TelephoneValidation(string verifiableTelephone)
        {
            if (string.IsNullOrWhiteSpace(verifiableTelephone))
                return ValidationResult.Fail("Телефон не может быть пустым");

            if (!Regex.IsMatch(verifiableTelephone, _telephonePattern))
                return ValidationResult.Fail("Неверный формат телефона. Введите номер в формате: +7 (XXX) XXX-XX-XX");

            return ValidationResult.Ok();
        }

        public static ValidationResult StudyGroupValidation(string verifiableStudyGroup)
        {
            //TODO: добавить валидацию бы

            return ValidationResult.Ok();
        }

        public static ValidationResult PasswordValidation(string verifiablePassword)
        {
            if (string.IsNullOrWhiteSpace(verifiablePassword))
                return ValidationResult.Fail("Пароль не может быть пустым");

            if (verifiablePassword.Length < 8)
                return ValidationResult.Fail("Длина пароля не может быть меньше 8 символов");

            return ValidationResult.Ok();
        }
    }
}
