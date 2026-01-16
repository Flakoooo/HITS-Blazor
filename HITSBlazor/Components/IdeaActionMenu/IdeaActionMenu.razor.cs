using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HITSBlazor.Components.IdeaActionMenu
{
    public partial class IdeaActionMenu : IDisposable
    {
        [Parameter]
        public Guid IdeaId { get; set; }

        [Parameter]
        public EventCallback<Guid> OnView { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        private ElementReference _containerRef;
        private DotNetObjectReference<IdeaActionMenu>? _dotNetRef;
        private bool IsOpen { get; set; }
        private bool _isDisposed;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (IsOpen)
            {
                _dotNetRef?.Dispose();
                _dotNetRef = DotNetObjectReference.Create(this);
                await RegisterClickOutside();
            }
            else
            {
                await UnregisterClickOutside();
            }
        }

        private async Task RegisterClickOutside()
        {
            if (_isDisposed || _dotNetRef == null) return;

            try
            {
                await JSRuntime.InvokeVoidAsync(
                    "dropdownManager.registerClickOutside",
                    _containerRef,
                    _dotNetRef
                );
            }
            catch (ObjectDisposedException)
            {
                
            }
        }

        private async Task UnregisterClickOutside()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync(
                    "dropdownManager.unregisterClickOutside",
                    _containerRef
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unregistering click outside: {ex.Message}");
            }
        }

        private void ToggleMenu() => IsOpen = !IsOpen;

        [JSInvokable]
        public void CloseDropdown()
        {
            if (_isDisposed) return;

            IsOpen = false;
            StateHasChanged();
        }

        private async void OnViewClick()
        {
            IsOpen = false;
            await OnView.InvokeAsync(IdeaId);
        }

        private string GetMenuPosition() 
            => IsOpen ? "margin-top: 2.5rem; margin-left: 1.0rem;" : string.Empty;

        public void Dispose()
        {
            _isDisposed = true;

            try
            {
                _ = UnregisterClickOutside();
            }
            catch { }

            _dotNetRef?.Dispose();
            _dotNetRef = null;
        }
    }
}
