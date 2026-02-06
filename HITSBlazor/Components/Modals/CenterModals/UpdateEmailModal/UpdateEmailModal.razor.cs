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

        private Guid _verificationGuid;
        private string _emailValue = string.Empty;
        private string _verificationCode = string.Empty;

        private async Task SendNewEmailRequest()
        {
            _isLoading = true;

            _verificationGuid = await ProfileService.SendUpdateEmailRequestAsync(_emailValue);
            if (_verificationGuid != Guid.Empty)
                _isEmailRequestSended = true;

            _isLoading = false;
        }

        private async Task VirificateEmailRequest()
        {
            _isLoading = true;

            if (await ProfileService.UpdateEmailConfirmAsync(_verificationGuid, _verificationCode))
                ModalService.Close(ModalType.Center);

            _isLoading = false;
        }
    }
}
