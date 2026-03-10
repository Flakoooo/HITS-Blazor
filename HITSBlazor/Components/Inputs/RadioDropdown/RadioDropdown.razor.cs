using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace HITSBlazor.Components.Inputs.RadioDropdown
{
    public partial class RadioDropdown<T> : IAsyncDisposable
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

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

        private ElementReference _inputRef;
        private DotNetObjectReference<RadioDropdown<T>>? _dotNetHelper;

        private bool _isOpen = false;

        private List<T> _values = [];
        private string _radioGroupName = typeof(T).Name;
        private string _searchText = string.Empty;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _dotNetHelper = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("listDropdown.registerClickOutside", _inputRef, _dotNetHelper);
                _values = [.. AllValues];
            }
        }

        private async Task OpenDropdown()
        {
            if (!_isOpen) _isOpen = true;
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

        public async ValueTask DisposeAsync()
        {
            if (_dotNetHelper != null)
            {
                await JSRuntime.InvokeVoidAsync("listDropdown.unregisterClickOutside", _inputRef);
                _dotNetHelper.Dispose();
            }
        }
    }
}
