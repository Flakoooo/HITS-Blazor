using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace HITSBlazor.Pages
{
    [AllowAnonymous]
    [Route("/register")]
    public partial class Register : ComponentBase
    {
        private RegisterRequest registerRequest = new();
        private bool isLoading;
        private string? emailFromQuery;

        [Parameter]
        [SupplyParameterFromQuery(Name = "email")]
        public string? Email { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "code")]
        public string? Code { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [Inject]
        private NotificationService NotificationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            // Если email передан в query string, устанавливаем его
            if (!string.IsNullOrEmpty(Email))
            {
                registerRequest.Email = Email;
            }
        }

        private async Task HandleRegister()
        {
            if (isLoading) return;

            isLoading = true;

            try
            {
                // Ручная валидация как в JavaScript
                var validationErrors = new List<string>();

                // Валидация email (если не передан в query string)
                if (string.IsNullOrEmpty(registerRequest.Email))
                {
                    validationErrors.Add("Пожалуйста, заполните email");
                }
                else if (!IsValidEmail(registerRequest.Email))
                {
                    validationErrors.Add("Неверно введена почта");
                }

                // Валидация имени
                if (string.IsNullOrEmpty(registerRequest.FirstName))
                {
                    validationErrors.Add("Имя обязательно");
                }
                else if (!Regex.IsMatch(registerRequest.FirstName, @"^([А-ЯЁA-Z][а-яёa-z]{1,})|([A-Z][a-z]{1,})$"))
                {
                    validationErrors.Add("Неверно введено имя");
                }

                // Валидация фамилии
                if (string.IsNullOrEmpty(registerRequest.LastName))
                {
                    validationErrors.Add("Фамилия обязательна");
                }
                else if (!Regex.IsMatch(registerRequest.LastName, @"^([А-ЯЁA-Z][а-яёa-z]{1,})|([A-Z][a-z]{1,})$"))
                {
                    validationErrors.Add("Неверно введена фамилия");
                }

                // Валидация телефона
                if (string.IsNullOrEmpty(registerRequest.Telephone))
                {
                    validationErrors.Add("Телефон обязателен");
                }
                else if (!Regex.IsMatch(registerRequest.Telephone, @"^\+\d{1,3}\s\(\d{3}\)\s\d{3}-\d{2}-\d{2}$"))
                {
                    validationErrors.Add("Неверно введен номер телефона");
                }

                // Валидация пароля
                if (string.IsNullOrEmpty(registerRequest.Password))
                {
                    validationErrors.Add("Пароль обязателен");
                }
                else if (registerRequest.Password.Length < 8)
                {
                    validationErrors.Add("Пароль должен состоять как минимум из 8 символов");
                }

                if (validationErrors.Any())
                {
                    NotificationService.ShowError(string.Join(" ", validationErrors));
                    isLoading = false;
                    return;
                }

                // Проверка кода приглашения (если требуется)
                if (!string.IsNullOrEmpty(Code) && Code != "valid-invitation-code") // Заменить на реальную проверку
                {
                    NotificationService.ShowError("Неверный код приглашения");
                    isLoading = false;
                    return;
                }

                // Имитация регистрации
                await Task.Delay(1500);

                // Успешная регистрация
                NotificationService.ShowSuccess("Регистрация успешна!");

                // Редирект на страницу логина через 2 секунды
                await Task.Delay(2000);
                Navigation.NavigateTo("/login");
            }
            catch (Exception ex)
            {
                NotificationService.ShowError($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void HandlePhoneInput(ChangeEventArgs e)
        {
            var value = e.Value?.ToString() ?? "";
            var cleaned = new string(value.Where(char.IsDigit).ToArray());

            if (string.IsNullOrEmpty(cleaned))
            {
                registerRequest.Telephone = "";
                return;
            }

            // Форматирование телефона как в JavaScript
            var result = "+";

            if (cleaned.StartsWith("8") || value.Contains("+8"))
            {
                result = "";
            }

            for (int i = 0; i < cleaned.Length && i < 11; i++)
            {
                switch (i)
                {
                    case 0:
                        result += PrefixNumber(cleaned[i]);
                        continue;
                    case 4:
                        result += ") ";
                        break;
                    case 7:
                        result += "-";
                        break;
                    case 9:
                        result += "-";
                        break;
                }
                result += cleaned[i];
            }

            registerRequest.Telephone = result;
        }

        private string PrefixNumber(char digit)
        {
            return digit switch
            {
                '7' => "7 (",
                '8' => "8 (",
                '9' => "7 (9",
                _ => "7 ("
            };
        }
    }
}
