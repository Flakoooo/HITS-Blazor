using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.Components.ModalHeader
{
    public partial class ModalHeader
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public string Title { get; set; } = string.Empty;

        private void CloseModal() => ModalService.Close(ModalType.RightSide);
    }
}
