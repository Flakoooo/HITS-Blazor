using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Filters.RadioFilter
{
    public partial class RadioFilter
    {
        [Parameter]
        public bool? Value { get; set; }

        [Parameter]
        public string Label { get; set; } = string.Empty;

        [Parameter]
        public string Value1Text { get; set; } = string.Empty;

        [Parameter]
        public string Value2Text { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<bool?> ValueChanged { get; set; }

        [Parameter]
        public EventCallback OnFilterChanged { get; set; }

        private bool CheckState(bool state) => Value == state;

        private async Task ToggleValue(bool value)
        {
            Value = Value == value ? null : value;

            await ValueChanged.InvokeAsync(Value);
            await OnFilterChanged.InvokeAsync();
            StateHasChanged();
        }
    }
}
