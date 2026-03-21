using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Filters.RadioFilter
{
    public partial class RadioFilter<T>
    {
        [Parameter]
        public string Label { get; set; } = string.Empty;

        [Parameter]
        public List<T> AllValues { get; set; } = [];

        [Parameter]
        public T? Value { get; set; }

        [Parameter]
        public EventCallback<T?> ValueChanged { get; set; }

        [Parameter]
        public EventCallback OnFilterChanged { get; set; }

        private bool CheckState(T state) => Value == state;

        private async Task ToggleValue(T value)
        {
            Value = Value == value ? null : value;

            await ValueChanged.InvokeAsync(Value);
            await OnFilterChanged.InvokeAsync();
            StateHasChanged();
        }
    }
}
