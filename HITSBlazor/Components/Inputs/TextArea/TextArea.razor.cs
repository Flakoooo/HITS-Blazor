using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Inputs.TextArea
{
    public partial class TextArea
    {
        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public string HintText { get; set; } = string.Empty;

        [Parameter]
        public string Placeholder { get; set; } = string.Empty;

        [Parameter]
        public string Value { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public bool NeedValidation { get; set; } = false;

        [Parameter]
        public string? ErrorMessage { get; set; } = "Поле не заполнено";

        private bool _showError = false;

        protected override void OnParametersSet()
        {
            _showError = NeedValidation && string.IsNullOrWhiteSpace(Value);
            StateHasChanged();
        }

        private async Task OnInputChanged(ChangeEventArgs e)
        {
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(e.Value?.ToString());
        }
    }
}
