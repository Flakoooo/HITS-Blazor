using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.UpdateEmailModal
{
    public partial class UpdateEmailModal
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private bool _isLoading = false;
        private bool _isEmailRequestSended = false;

        private string _emailValue = string.Empty;
        private string _verificationCode = string.Empty;

        private void SendNewEmailRequest()
        {
            _isLoading = true;

            _isEmailRequestSended = true;

            _isLoading = false;
        }

        private void VirificateEmailRequest()
        {
            _isLoading = true;

            _isEmailRequestSended = false;

            _isLoading = false;
            ModalService.Close();
        }
    }
}
