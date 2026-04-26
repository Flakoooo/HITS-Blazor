using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Services.Debounce;
using HITSBlazor.Services.Skills;
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

        [Parameter]
        public Func<int, string?, Task<ListDataResponse<T>>>? DataLoaderMethod { get; set; }

        [Parameter]
        public int DebounceDelay { get; set; } = 0;

        private bool IsOnLoadingNow => _isLoading || IsLoading;

        private bool _isLoading;

        private List<T> _values = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (DataLoaderMethod is not null)
            {
                await LoadCheckboxDataAsync();
                MarkAsInitialized();
            }

            _isLoading = false;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (DataLoaderMethod is not null)
            {
                ResetPagination();
                await LoadCheckboxDataAsync();
            }
            else
            {
                if (!SearchTextChanged.HasDelegate && _values.Count == 0)
                {
                    _values = AllValues.ToList();
                }
                else
                {
                    _values = string.IsNullOrWhiteSpace(SearchText)
                        ? AllValues.ToList()
                        : AllValues.Where(v => v.MatchesSearch(SearchText)).ToList();
                }
            }
        }

        protected override async Task OnLoadMoreItemsAsync()
            => await LoadCheckboxDataAsync(append: true);

        protected override int GetCurrentItemsCount() => _values.Count;

        private async Task LoadCheckboxDataAsync(bool append = false)
        {
            if (DataLoaderMethod is not null)
            {
                await LoadDataAsync(
                    AllValues,
                    () => DataLoaderMethod.Invoke(_currentPage, SearchText),
                    append: append
                );

                _values = AllValues.ToList();
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
