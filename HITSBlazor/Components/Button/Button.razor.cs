using HITSBlazor.Components.Typography;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Button
{
    public partial class Button
    {
        [Parameter]
        public ButtonType BtnType { get; set; } = ButtonType.Button;

        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public string LoadingWidth { get; set; } = "w-100";

        [Parameter]
        public bool InProgress { get; set; } = false;

        [Parameter]
        public EventCallback OnButtonClick { get; set; }

        [Parameter]
        public string Text { get; set; } = string.Empty;

        [Parameter]
        public string LoadingText { get; set; } = string.Empty;

        [Parameter]
        public int? TextSize { get; set; }

        [Parameter]
        public TextColor? TextColor { get; set; }

        [Parameter]
        public string TextCustomClass { get; set; } = string.Empty;

        [Parameter]
        public string IconStyle { get; set; } = string.Empty;

        [Parameter]
        public string IconSize { get; set; } = "fs-5";

        [Parameter]
        public ButtonVariant? Variant { get; set; }

        [Parameter]
        public string? ButtonClass { get; set; }

        [Parameter]
        public string Style { get; set; } = string.Empty;

        private async Task ButtonClick() => await OnButtonClick.InvokeAsync();

        private string GetClass()
        {
            var classes = new List<string>();

            if (Variant.HasValue)
                classes.Add($"btn-{Variant.Value.ToString().ToLower()}");

            if (!string.IsNullOrWhiteSpace(ButtonClass))
                classes.Add(ButtonClass);

            if (classes.Count == 0)
                return "btn d-flex";

            return $"btn d-flex {string.Join(" ", classes)}";
        }
    }
}
