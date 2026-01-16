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
        private ElementReference _triggerRef;
        private DotNetObjectReference<IdeaActionMenu>? _dotNetRef;
        private bool IsOpen { get; set; }
        private bool _isDisposed;
        private string _menuStyle = "";

        protected override void OnInitialized()
        {
            _dotNetRef = DotNetObjectReference.Create(this);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (IsOpen)
            {
                await CalculateMenuPosition();

                await RegisterClickOutside();
            }
            else
            {
                await UnregisterClickOutside();
            }
        }

        private async Task CalculateMenuPosition()
        {
            try
            {
                var shouldShowAbove = await JSRuntime.InvokeAsync<bool>(
                    "dropdownManager.shouldShowAbove",
                    _triggerRef
                );

                var styleBuilder = new System.Text.StringBuilder();
                styleBuilder.Append("position: absolute; ");

                if (shouldShowAbove)
                {
                    styleBuilder.Append("bottom: 100%; ");
                    styleBuilder.Append("left: 0; ");
                    styleBuilder.Append("margin-bottom: 5px; ");
                }
                else
                {
                    styleBuilder.Append("top: 100%; ");
                    styleBuilder.Append("left: 0; ");
                    styleBuilder.Append("margin-top: 5px; ");
                }

                styleBuilder.Append("width: 160px; ");
                styleBuilder.Append("z-index: 1000; ");

                _menuStyle = styleBuilder.ToString();
                StateHasChanged();
            }
            catch (Exception)
            {
                _menuStyle = "position: absolute; top: 100%; left: 0; margin-top: 5px; width: 160px; z-index: 1000;";
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
                    _dotNetRef,
                    IdeaId.ToString()
                );
            }
            catch { }
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

            if (IsOpen)
                await JSRuntime.InvokeVoidAsync("dropdownManager.closeOtherMenus", IdeaId.ToString());

            await Task.CompletedTask;
        }

        [JSInvokable]
        public async Task CloseDropdown()
        {
            if (_isDisposed) return;

            IsOpen = false;
            _menuStyle = "";
            StateHasChanged();
            await Task.CompletedTask;
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
            if (_isDisposed) return;
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
