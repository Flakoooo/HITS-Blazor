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
        private bool _submitted = false;
        private bool _isEmailRequestSended = false;

        private Guid _verificationGuid;
        private string EmailValue { get; set; } = string.Empty;
        private string VerificationCode { get; set; } = string.Empty;

        private async Task SendNewEmailRequest()
        {
            _isLoading = true;
            _submitted = false;

            _verificationGuid = await ProfileService.SendUpdateEmailRequestAsync(EmailValue);
            if (_verificationGuid != Guid.Empty)
            {
                _submitted = false;
                _isEmailRequestSended = true;
            }
            else
            {
                _submitted = true;
            }

            _isLoading = false;
        }

        private async Task VirificateEmailRequest()
        {
            _isLoading = true;

            if (await ProfileService.UpdateEmailConfirmAsync(_verificationGuid, VerificationCode))
            {
                _submitted = false;
                await ModalService.Close(ModalType.Center);
            }
            else
            {
                _submitted = true;
            }

            _isLoading = false;
        }
    }
}
