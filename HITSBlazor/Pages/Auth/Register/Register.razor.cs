using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Invitation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Auth.Register
{
    [AllowAnonymous]
    [Route("/register/{invitationId}")]
    public partial class Register : ComponentBase
    {
        private RegisterModel _registerModel = new();

        private bool _submitted = false;
        private bool _submitting = false;

        private string _phoneInput = "";
        private string _rawDigits = "";

        [Parameter]
        public string InvitationId { get; set; } = string.Empty;

        private bool IsEmailDisabled { get; set; } = true;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private IInvitationService InvitationService { get; set; } = null!;

        [Inject]
        private GlobalNotificationService NotificationService { get; set; } = null!;

        //TODO: Сделать так, чтобы выбрасывало из регистрации, если не найдено id приглашения

        protected override async Task OnInitializedAsync()
        {
            if (!Guid.TryParse(InvitationId, out Guid guid))
            {
                IsEmailDisabled = false;
                return;
            }

            var invitedEmail = await InvitationService.GetEmailById(guid);
            if (string.IsNullOrWhiteSpace(invitedEmail))
            {
                IsEmailDisabled = false;
            }
            else
            {
                _registerModel.Email = invitedEmail;
                IsEmailDisabled = true;
            }
        }

        private async Task HandlePhoneInput(string value)
        {
            if (value.Length < _phoneInput.Length && _phoneInput.Last() != '(')
            {
                _rawDigits = _rawDigits[..^1];
                var newValue = _phoneInput[..^1];

                if (newValue.Last() == '-')
                    _registerModel.Telephone = _phoneInput = newValue[..^1];
                else if (newValue.Last() == ' ')
                    _registerModel.Telephone = _phoneInput = newValue[..^2];
                else
                    _registerModel.Telephone = _phoneInput = newValue;
            }
            else if (value.Length > _phoneInput.Length)
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

                _rawDigits = digits;
                _registerModel.Telephone = _phoneInput = result;
            }
        }

        private async Task HandleRegister()
        {
            if (_submitting) return;

            _submitting = true;
            _submitted = false;

            if (Guid.TryParse(InvitationId, out Guid guid) && await AuthService.RegistrationAsync(_registerModel, guid))
            {
                _registerModel = new RegisterModel();
                Navigation.NavigateTo("/redirect");
                return;
            }

            _submitted = true;
            _submitting = false;
        }
    }
}
