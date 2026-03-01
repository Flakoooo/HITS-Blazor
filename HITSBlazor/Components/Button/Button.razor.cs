using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Button
{
    public partial class Button
    {
        [Parameter]
        public ButtonType BtnType { get; set; } = ButtonType.Button;

        [Parameter]
        public bool? IsLoading { get; set; } = false;

        [Parameter]
        public EventCallback OnButtonClick { get; set; }

        [Parameter]
        public string Text { get; set; } = string.Empty;

        [Parameter]
        public string? LoadingText { get; set; }

        [Parameter]
        public ButtonVariant Variant { get; set; } = ButtonVariant.Primary;

        [Parameter]
        public string ButtonClass { get; set; } = string.Empty;

        private async Task ButtonClick() => await OnButtonClick.InvokeAsync();
    }
}
