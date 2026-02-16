using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HITSBlazor.Services.ActionMenus
{
    public class ActionMenuService(IJSRuntime jsRuntime) : IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime = jsRuntime;
        private DotNetObjectReference<BaseActionMenuComponent>? _currentMenuRef;
        private string? _currentMenuId;

        public async Task RegisterCurrentMenuAsync(
            string menuId,
            ElementReference triggerRef,
            ElementReference menuRef,
            DotNetObjectReference<BaseActionMenuComponent> dotNetRef)
        {
            if (_currentMenuRef != null && _currentMenuRef != dotNetRef)
            {
                try
                {
                    await _currentMenuRef.Value.CloseDropdown();
                }
                catch { }
            }

            _currentMenuRef = dotNetRef;
            _currentMenuId = menuId;

            await _jsRuntime.InvokeVoidAsync(
                "menuDropdown.registerClickOutside",
                triggerRef,
                menuRef,
                dotNetRef);
        }

        public async Task UnregisterCurrentMenuAsync(string menuId, DotNetObjectReference<BaseActionMenuComponent> dotNetRef)
        {
            if (_currentMenuRef == dotNetRef && _currentMenuId == menuId)
            {
                _currentMenuRef = null;
                _currentMenuId = null;

                await _jsRuntime.InvokeVoidAsync("menuDropdown.unregisterClickOutside");
            }
        }

        public async Task EnsureMenuVisibleAsync(ElementReference menuRef)
        {
            await _jsRuntime.InvokeVoidAsync("menuDropdown.ensureMenuVisible", menuRef);
        }

        public async Task StartMenuAnimationAsync(ElementReference menuRef)
        {
            await _jsRuntime.InvokeVoidAsync("menuDropdown.startMenuAnimation", menuRef);
        }

        public async Task HideMenuAsync(ElementReference menuRef)
        {
            await _jsRuntime.InvokeVoidAsync("menuDropdown.hideMenu", menuRef);
        }

        public async ValueTask DisposeAsync()
        {
            if (_currentMenuRef != null)
            {
                try
                {
                    await _currentMenuRef.Value.CloseDropdown();
                }
                catch { }

                _currentMenuRef = null;
                _currentMenuId = null;
            }
        }
    }
}
