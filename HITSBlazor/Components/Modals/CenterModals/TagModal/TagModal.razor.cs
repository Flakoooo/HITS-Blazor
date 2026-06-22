using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Requests;
using HITSBlazor.Services;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Tags;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.CenterModals.TagModal
{
    public partial class TagModal
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private ITagService TagService { get; set; } = null!;

        [Inject]
        private GlobalNotificationService NotificationService { get; set; } = null!;

        [Parameter]
        public Tag? Tag { get; set; }

        private bool _isLoading = true;
        private bool _submitting = false;

        private string TagName { get; set; } = string.Empty;
        private string TagColor { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (Tag is not null)
            {
                TagName = Tag.Name;
                TagColor = Tag.Color;
            }

            _isLoading = false;
        }

        private bool CheckValidValues()
        {
            if (string.IsNullOrWhiteSpace(TagName)) return false;
            if (string.IsNullOrWhiteSpace(TagColor)) return false;

            return true;
        }

        private async Task SendTag()
        {
            _submitting = true;

            if (!CheckValidValues())
            {
                NotificationService.ShowError("Заполнены не все поля");
                _submitting = false;
                return;
            }

            bool result = Tag is not null 
                ? await TagService.UpdateTagAsync(
                    Tag.Id,
                    new UpdateTagRequest { Name = TagName, Color = TagColor }
                )
                : (await TagService.CreateNewTagAsync(
                    TagName,
                    TagColor,
                    true
                )) is not null;

            if (result) await ModalService.Close(ModalType.Center);

            _submitting = false;
        }
    }
}
