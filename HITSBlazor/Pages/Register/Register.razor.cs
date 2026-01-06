using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Invitation;
using HITSBlazor.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Register
{
    [AllowAnonymous]
    [Route("/register/{invitationId}")]
    public partial class Register : ComponentBase
    {
        private RegisterModel registerModel = new();
        private bool isLoading;

        private string phoneInput = "";
        private string rawDigits = "";

        [Parameter]
        public Guid InvitationId { get; set; }

        private bool IsEmailDisabled { get; set; } = true;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private InvitationApi InvitationApi { get; set; } = null!;

        [Inject]
        private GlobalNotificationService NotificationService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            ServiceResponse<string> response = await InvitationApi.GetEmailByInvitationId(InvitationId);
            if (response.IsSuccess && response.Response is not null)
            {
                registerModel.Email = response.Response;
                IsEmailDisabled = true;
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

            if (await AuthService.RegistrationAsync(registerModel, InvitationId))
            {
                registerModel = new RegisterModel();
                Navigation.NavigateTo("/redirect");
            }

            isLoading = false;
        }
    }
}
