using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Filters.CheckboxFilter
{
    public partial class CheckboxFilter<T>
    {
        [Parameter]
        public bool IsLoading { get; set; } = false;

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

        [Parameter]
        public string SearchText { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<string> SearchTextChanged { get; set; }

        private List<T> _values = [];

        protected override void OnParametersSet()
        {
            if (!SearchTextChanged.HasDelegate)
            {
                if (_values.Count == 0) _values = [.. AllValues];
            }
            else
            {
                _values = string.IsNullOrWhiteSpace(SearchText)
                    ? [.. AllValues]
                    : [.. AllValues.Where(v => v.MatchesSearch(SearchText))];
            }
        }

        private async Task SearchData(string value)
        {
            SearchText = value;
            await SearchTextChanged.InvokeAsync(value);
        }

        private async Task SelectValue(T value)
        {
            if (!SelectedValues.Add(value))
                SelectedValues.Remove(value);

            await SelectedValuesChanged.InvokeAsync(SelectedValues);
            await FilterChanged.InvokeAsync();
        }
    }
}
