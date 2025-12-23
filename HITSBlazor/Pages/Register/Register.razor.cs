using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Invitation;
using HITSBlazor.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace HITSBlazor.Pages.Register
{
    [AllowAnonymous]
    [Route("/register/{invitationId}")]
    public partial class Register : ComponentBase
    {
        private RegisterModel registerModel = new();
        private bool isLoading;

        private readonly string telephonePattern = @"^\+7\s\(\d{3}\)\s\d{3}-\d{2}-\d{2}$";
        private readonly string namePattern = @"^[А-ЯЁA-Z][а-яёa-z]+(-[А-ЯЁA-Z][а-яёa-z]+)?$";

        private string phoneInput = "";
        private string rawDigits = "";

        [Parameter]
        public string InvitationId { get; set; } = string.Empty;

        private bool IsEmailDisabled { get; set; } = true;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private InvitationApi InvitationApi { get; set; } = null!;

        [Inject]
        private NotificationService NotificationService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            if (Guid.TryParse(InvitationId, out Guid guid))
            {
                ServiceResponse<string> response = await InvitationApi.GetEmailByInvitationId(guid);
                if (response.IsSuccess && response.Response is not null)
                {
                    registerModel.Email = response.Response;
                    IsEmailDisabled = true;
                }
                else IsEmailDisabled = false;
            }
            else IsEmailDisabled = false;
        }

        private async Task HandlePhoneInput(string value)
        {
            if (value.Length < phoneInput.Length && phoneInput.Last() != '(')
            {
                rawDigits = rawDigits[..^1];
                var newValue = phoneInput[..^1];

                if (newValue.Last() == '-')
                    registerModel.Telephone = phoneInput = newValue[..^1];
                else if (newValue.Last() == ' ')
                    registerModel.Telephone = phoneInput = newValue[..^2];
                else
                    registerModel.Telephone = phoneInput = newValue;
            }
            else if (value.Length > phoneInput.Length)
            {
                var digits = new string([.. value.Where(char.IsDigit)]);

                var result = "+";
                for (int i = 0; i < digits.Length; ++i)
                {
                    _ = i switch
                    {
                        0 => result += "7 (",
                        4 => result += $") {digits[i]}",
                        7 or 9 => result += $"-{digits[i]}",
                        _ => result += digits[i]
                    };
                }

                rawDigits = digits;
                registerModel.Telephone = phoneInput = result;
            }
        }

        private async Task HandleRegister()
        {
            if (isLoading) return;

            isLoading = true;

            try
            {
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

                    NotificationService.ShowError(errorMessage);
                    isLoading = false;
                    return;
                }

                var email = registerModel.Email.Trim();
                var atIndex = email.IndexOf('@');

                if (atIndex <= 0 || atIndex == email.Length - 1 || email.Count(c => c == '@') != 1)
                {
                    NotificationService.ShowError("Неверный формат почты");
                    isLoading = false;
                    return;
                }

                if (!Regex.IsMatch(registerModel.FirstName, namePattern))
                {
                    NotificationService.ShowError("Имя должно начинаться с заглавной буквы и содержать только буквы (и дефис для двойных имён)");
                    isLoading = false;
                    return;
                }

                if (!Regex.IsMatch(registerModel.LastName, namePattern))
                {
                    NotificationService.ShowError("Фамилия должна начинаться с заглавной буквы и содержать только буквы (и дефис для двойных фамилий)");
                    isLoading = false;
                    return;
                }

                if (!string.IsNullOrWhiteSpace(registerModel.Telephone) && !Regex.IsMatch(registerModel.Telephone, telephonePattern))
                {
                    NotificationService.ShowError("Неверный формат телефона. Введите номер в формате: +7 (XXX) XXX-XX-XX");
                    isLoading = false;
                    return;
                }

                if (registerModel.Password.Length < 8)
                {
                    NotificationService.ShowError("Длина пароля не может быть меньше 8 символов");
                    isLoading = false;
                    return;
                }

                var result = await AuthService.RegisterAsync(registerModel);
                if (result.IsSuccess)
                {
                    registerModel = new RegisterModel();
                    NotificationService.ShowSuccess(result.Message);
                    Navigation.NavigateTo("/", true);
                }
                else
                {
                    NotificationService.ShowError(
                        result.Message ?? "Вы указали неверные данные для регистрации. Попробуйте снова."
                    );
                }
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
    }
}
