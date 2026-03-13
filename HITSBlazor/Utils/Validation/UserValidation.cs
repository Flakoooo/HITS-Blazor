using HITSBlazor.Pages.Auth.Register;
using HITSBlazor.Services;
using System.Text.RegularExpressions;

namespace HITSBlazor.Utils.Validation
{
    public class UserValidation
    {
        private const string _namePattern = @"^[А-ЯЁA-Z][а-яёa-z]+(-[А-ЯЁA-Z][а-яёa-z]+)?$";
        private const string _telephonePattern = @"^\+7\s\(\d{3}\)\s\d{3}-\d{2}-\d{2}$";
        //TODO: добавить паттерн для учебных групп

        public static ValidationEvaluation EmailValidation(string verifiableEmail)
        {
            var result = new ValidationEvaluation();

            if (string.IsNullOrWhiteSpace(verifiableEmail))
            {
                result.IsValid = false;
                result.Message = "Почта не может быть пустой";
                return result;
            }

            var email = verifiableEmail.Trim();
            var atIndex = email.IndexOf('@');
            if (atIndex <= 0 || atIndex == email.Length - 1 || email.Count(c => c == '@') != 1)
            {
                result.IsValid = false;
                result.Message = "Неверный формат почты";
                return result;
            }

            return result;
        }

        public static ValidationEvaluation FirstNameValidation(string verifiableFirstName)
        {
            var result = new ValidationEvaluation();

            if (string.IsNullOrWhiteSpace(verifiableFirstName))
            {
                result.IsValid = false;
                result.Message = "Имя не может быть пустым";
                return result;
            }

            if (!Regex.IsMatch(verifiableFirstName, _namePattern))
            {
                result.IsValid = false;
                result.Message = "Имя должно начинаться с заглавной буквы и содержать только буквы (и дефис для двойных имён)";
                return result;
            }

            return result;
        }

        public static ValidationEvaluation LastNameValidation(string verifiableLastName)
        {
            var result = new ValidationEvaluation();

            if (string.IsNullOrWhiteSpace(verifiableLastName))
            {
                result.IsValid = false;
                result.Message = "Фамилия не может быть пустой";
                return result;
            }

            if (!Regex.IsMatch(verifiableLastName, _namePattern))
            {
                result.IsValid = false;
                result.Message = "Фамилия должна начинаться с заглавной буквы и содержать только буквы (и дефис для двойных фамилий)";
                return result;
            }

            return result;
        }

        public static ValidationEvaluation TelephoneValidation(string verifiableTelephone)
        {
            var result = new ValidationEvaluation();

            if (string.IsNullOrWhiteSpace(verifiableTelephone))
            {
                return result;
            }

            if (!Regex.IsMatch(verifiableTelephone, _telephonePattern))
            {
                result.IsValid = false;
                result.Message = "Неверный формат телефона. Введите номер в формате: +7 (XXX) XXX-XX-XX";
                return result;
            }

            return result;
        }

        public static ValidationEvaluation StudyGroupValidation(string verifiableStudyGroup)
        {
            var result = new ValidationEvaluation();

            //TODO: добавить валидацию бы

            return result;
        }

        public static ValidationEvaluation PasswordValidation(string verifiablePassword)
        {
            var result = new ValidationEvaluation();

            if (string.IsNullOrWhiteSpace(verifiablePassword))
            {
                result.IsValid = false;
                result.Message = "Пароль не может быть пустым";
                return result;
            }

            if (verifiablePassword.Length < 8)
            {
                result.IsValid = false;
                result.Message = "Длина пароля не может быть меньше 8 символов";
                return result;
            }

            return result;
        }
    }
}
