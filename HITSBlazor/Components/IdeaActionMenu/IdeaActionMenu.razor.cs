using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HITSBlazor.Components.IdeaActionMenu
{
    public partial class IdeaActionMenu : IDisposable, IAsyncDisposable
    {
        [Parameter]
        public Guid IdeaId { get; set; }

        [Parameter]
        public EventCallback<Guid> OnView { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        private ElementReference _containerRef;
        private ElementReference _triggerRef;
        private DotNetObjectReference<IdeaActionMenu>? _dotNetRef;
        private bool IsOpen { get; set; }
        private bool _isDisposed;
        private string _menuStyle = "";
        private long? _resizeObserverId;
        private bool _isMenuAbove = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (IsOpen)
            {
                _dotNetRef?.Dispose();
                _dotNetRef = DotNetObjectReference.Create(this);

                await CalculateMenuPosition();

                await RegisterClickOutside();

                await RegisterResizeObserver();
            }
            else
            {
                await UnregisterClickOutside();
                await UnregisterResizeObserver();
            }
        }

        private async Task CalculateMenuPosition()
        {
            try
            {
                var positionInfo = await JSRuntime.InvokeAsync<PositionInfo>(
                    "dropdownManager.calculateMenuPosition",
                    _triggerRef
                );

                if (positionInfo == null) return;

                _isMenuAbove = positionInfo.ShowAbove;

                var styleBuilder = new System.Text.StringBuilder();
                styleBuilder.Append("position: absolute; ");

                if (_isMenuAbove)
                {
                    styleBuilder.Append($"bottom: 100%; ");
                    styleBuilder.Append($"left: {positionInfo.Left}px; ");
                    styleBuilder.Append("margin-bottom: 5px; ");
                }
                else
                {
                    styleBuilder.Append($"top: 100%; ");
                    styleBuilder.Append($"left: {positionInfo.Left}px; ");
                    styleBuilder.Append("margin-top: 5px; ");
                }

                styleBuilder.Append($"max-width: min(200px, {positionInfo.MaxWidth}px); ");
                styleBuilder.Append("z-index: 1000; ");

                _menuStyle = styleBuilder.ToString();
                StateHasChanged();
            }
            catch (Exception)
            {
                _menuStyle = "position: absolute; top: 100%; left: 0; margin-top: 5px; z-index: 1000;";
            }
        }

        private async Task RegisterResizeObserver()
        {
            try
            {
                if (_resizeObserverId is null && _dotNetRef is not null)
                {
                    _resizeObserverId = await JSRuntime.InvokeAsync<long>(
                        "dropdownManager.registerResizeObserver",
                        _containerRef,
                        _dotNetRef
                    );
                }
            }
            catch { }
        }

        private async Task UnregisterResizeObserver()
        {
            try
            {
                if (_resizeObserverId.HasValue)
                {
                    await JSRuntime.InvokeVoidAsync(
                        "dropdownManager.unregisterResizeObserver",
                        _resizeObserverId.Value
                    );
                    _resizeObserverId = null;
                }
            }
            catch { }
        }

        private async Task RegisterClickOutside()
        {
            if (_isDisposed || _dotNetRef is null) return;

            try
            {
                await JSRuntime.InvokeVoidAsync(
                    "dropdownManager.registerClickOutside",
                    _containerRef,
                    _dotNetRef
                );
            }
            catch (ObjectDisposedException) { }
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
            catch { }
        }

        private async Task ToggleMenu()
        {
            IsOpen = !IsOpen;
            if (!IsOpen) _menuStyle = "";

            await Task.CompletedTask;
        }

        [JSInvokable]
        public async Task CloseDropdown()
        {
            if (_isDisposed) return;

            IsOpen = false;
            _menuStyle = "";
            await UnregisterResizeObserver();
            StateHasChanged();
        }

        [JSInvokable]
        public async Task OnViewportChange()
        {
            if (IsOpen && !_isDisposed)
                await CalculateMenuPosition();
        }

        private async Task OnViewClick()
        {
            IsOpen = false;
            _menuStyle = "";
            await OnView.InvokeAsync(IdeaId);
        }

        private string GetMenuStyle() => _menuStyle;

        public void Dispose()
        {
            DisposeAsync().AsTask().Wait();
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            try
            {
                await UnregisterClickOutside();
                await UnregisterResizeObserver();
            }
            catch { }

            _dotNetRef?.Dispose();
            _dotNetRef = null;
        }
    }
}
