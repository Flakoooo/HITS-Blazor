using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Filters.CheckboxFilter
{
    public partial class CheckboxFilter<T>
    {
        [Parameter]
        public string Label { get; set; } = string.Empty;

        [Parameter]
        public List<T> AllValues { get; set; } = [];

        [Parameter]
        public HashSet<T> SelectedValues { get; set; } = [];

        [Parameter]
        public EventCallback<HashSet<T>> SelectedValuesChanged { get; set; }

        [Parameter]
        public EventCallback FilterChanged { get; set; }

        private async Task OnCheckboxChange(T value, bool isChecked)
        {
            if (isChecked)
                SelectedValues.Add(value);
            else
                SelectedValues.Remove(value);

            await SelectedValuesChanged.InvokeAsync(SelectedValues);
            await FilterChanged.InvokeAsync();
        }
    }
}
