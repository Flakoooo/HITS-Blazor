using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Inputs.Select
{
    public partial class Select
    {
        [Parameter]
        public string Label { get; set; } = string.Empty;

        [Parameter]
        public string Value { get; set; } = string.Empty;

        [Parameter]
        public List<KeyValuePair<int, string>> Options { get; set; } = [];

        [Parameter]
        public EventCallback<string> OnValueSelected { get; set; }

        private async Task OnOptionChanged(ChangeEventArgs e)
            => await OnValueSelected.InvokeAsync(e.Value?.ToString() ?? "");
    }
}
