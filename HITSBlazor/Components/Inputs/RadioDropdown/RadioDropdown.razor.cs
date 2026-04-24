using HITSBlazor.Models.Common.Responses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace HITSBlazor.Components.Inputs.RadioDropdown
{
    public partial class RadioDropdown<T>
    {
        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public string? Label { get; set; }

        [Parameter] 
        public string Placeholder { get; set; } = string.Empty;

        [Parameter]
        public string? HintText { get; set; }

        [Parameter]
        public T? Value { get; set; }

        [Parameter] 
        public EventCallback<T?> ValueChanged { get; set; }

        [Parameter]
        public List<T> AllValues { get; set; } = [];

        [Parameter]
        public bool IsDisabled { get; set; } = false;

        [Parameter]
        public Func<int, string?, Task<ListDataResponse<T>>>? DataLoaderMethod { get; set; }

        private ElementReference _inputRef;

        private bool _isOpen = false;

        private List<T> _values = [];
        private readonly string _radioGroupName = typeof(T).Name;
        private string _searchText = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            MarkAsInitialized();
        }

        protected override async Task AdditionalAfterRenderMethod()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("listDropdown.registerClickOutside", _inputRef, _dotNetHelper);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка регистрации clickOutside: {ex.Message}");
            }

            _values = [.. AllValues];
        }


        private async Task OpenDropdown()
        {
            if (!_isOpen)
            {
                _isOpen = true;
                await InvokeAsync(StateHasChanged);
            }
        }

        private void ToggleDropdown() => _isOpen = !_isOpen;

        [JSInvokable]
        public void CloseDropdown()
        {
            _isOpen = false;
            InvokeAsync(StateHasChanged);
        }

        private async Task OnSearch(ChangeEventArgs e)
        {
            _searchText = (e.Value?.ToString() ?? "").Trim();

            if (string.IsNullOrWhiteSpace(_searchText))
                _values = [.. AllValues];
            else
                _values = [.. AllValues.Where(v => v.MatchesSearch(_searchText))];

            await InvokeAsync(StateHasChanged);

        }

        private void OnInputKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Escape") _isOpen = false;
        }

        private async Task OnElementChange(T value)
        {
            Value = value;
            await ValueChanged.InvokeAsync(value);
        }

        private bool IsItemSelected(T value)
        {
            if (Value is null) return false;
            
            return Value.Equals(value);
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            try
            {
                if (_dotNetHelper != null)
                    await JSRuntime.InvokeVoidAsync("listDropdown.unregisterClickOutside", _inputRef);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при очистке RadioDropdown: {ex.Message}");
            }

            await ValueTask.CompletedTask;
        }

        private async Task LoadDataAsync(bool append = false)
        {
            if (DataLoaderMethod is not null)
            {
                if (!append)
                {
                    ResetPagination();
                    AllValues.Clear();
                }

                StateHasChanged();

                var listResponse = await DataLoaderMethod.Invoke(_currentPage, _searchText);

                _totalCount = listResponse.Count;
                if (listResponse.List.Count > 0)
                {
                    if (append)
                        AllValues.AddRange(listResponse.List);
                    else
                    {
                        AllValues.Clear();
                        AllValues.AddRange(listResponse.List);
                    }
                    _values = [.. AllValues];

                    IncrementPage();
                }

                StateHasChanged();
            }
        }

        protected override async Task OnLoadMoreItemsAsync()
        {
            await LoadDataAsync(append: true);
        }

        protected override int GetCurrentItemsCount() => _values.Count;
    }
}
