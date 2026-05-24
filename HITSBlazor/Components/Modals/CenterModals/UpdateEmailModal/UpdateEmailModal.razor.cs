using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Profiles;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.CenterModals.UpdateEmailModal
{
    public partial class UpdateEmailModal
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private IProfileService ProfileService { get; set; } = null!;

        private bool _isLoading = false;
        private bool _isEmailRequestSended = false;

        private readonly Dictionary<string, string> _errors = [];

        private Guid _verificationGuid;
        private string EmailValue { get; set; } = string.Empty;
        private string VerificationCode { get; set; } = string.Empty;

        private async Task SendNewEmailRequest()
        {
            _isLoading = true;
            _errors.Clear();

            if (string.IsNullOrWhiteSpace(EmailValue))
                _errors.Add("email", "Поле не может быть пустым");

            if (_errors.Count > 0) return;

            _verificationGuid = await ProfileService.SendUpdateEmailRequestAsync(EmailValue);
            if (_verificationGuid != Guid.Empty)
            {
                _isEmailRequestSended = true;
            }

            _isLoading = false;
        }

        private async Task VirificateEmailRequest()
        {
            _isLoading = true;
            _errors.Clear();

            if (string.IsNullOrWhiteSpace(VerificationCode))
                _errors.Add("code", "Поле не может быть пустым");

            if (string.IsNullOrWhiteSpace(EmailValue))
                _errors.Add("email", "Поле не может быть пустым");

            if (_errors.Count > 0) return;

            if (await ProfileService.UpdateEmailConfirmAsync(_verificationGuid, VerificationCode))
            {
                await ModalService.Close(ModalType.Center);
            }

            _isLoading = false;
        }
    }
}
