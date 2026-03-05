using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Inputs.Input
{
    public partial class Input
    {
        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public bool Hint { get; set; } = false;

        [Parameter]
        public string IconStyle { get; set; } = string.Empty;

        [Parameter]
        public string Placeholder { get; set; } = string.Empty;

        [Parameter]
        public string Value { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        private async Task OnInputChanged(ChangeEventArgs e)
        {
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(e.Value?.ToString());
        }
    }
}
