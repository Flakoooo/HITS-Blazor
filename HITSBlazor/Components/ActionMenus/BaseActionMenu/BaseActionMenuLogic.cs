using KristofferStrube.Blazor.Popper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HITSBlazor.Components.ActionMenus.BaseActionMenu
{
    public abstract class BaseActionMenuLogic : ComponentBase, IDisposable
    {
        [Parameter]
        public Guid ItemId { get; set; }

        [Parameter]
        public Strategy StrategyPosition { get; set; } = Strategy.Absolute;

        [Parameter]
        public EventCallback<TableActionContext> OnAction { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; } = null!;

        [Inject]
        protected Popper PopperService { get; set; } = null!;

        protected ElementReference _containerRef;
        protected ElementReference _triggerRef;
        protected ElementReference _menuRef;
        protected DotNetObjectReference<BaseActionMenuLogic>? _dotNetRef;
        protected Instance? _popperInstance;

        protected static List<Modifier> InstanceModifiers { get; } =
        [
            new(ModifierName.PreventOverflow)
            {
                Options = new
                {
                    boundary = "clippingParents",
                    rootBoundary = RootBoundary.Document,
                    padding = 8,
                    altAxis = true,
                    tether = false
                }
            },
            new(ModifierName.Flip)
            {
                Options = new
                {
                    fallbackPlacements = new[] { "top-start", "top-end", "bottom-start", "bottom-end" },
                    boundary = "clippingParents"
                }
            },
            new(ModifierName.Offset)
            {
                Options = new
                {
                    offset = new[] { 0, 8 },
                    boundary = "clippingParents",
                    rootBoundary = RootBoundary.Document
                }
            }
        ];

        protected bool IsOpen { get; set; }
        protected bool _isDisposed;

        protected override void OnInitialized()
        {
            _dotNetRef = DotNetObjectReference.Create(this);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (IsOpen)
            {
                await CreateOrUpdatePopper();
                await RegisterClickOutside();
            }
            else
            {
                await DestroyPopper();
                await UnregisterClickOutside();
            }
        }

        protected async Task CreateOrUpdatePopper()
        {
            await DestroyPopper();

            await JSRuntime.InvokeVoidAsync("menuDropdown.ensureMenuVisible", _menuRef);

            _popperInstance = await PopperService.CreatePopperAsync(
                reference: _triggerRef,
                popper: _menuRef,
                options: new Options
                {
                    Placement = Placement.BottomStart,
                    Strategy = StrategyPosition,
                    Modifiers = [.. InstanceModifiers]
                }
            );

            await _popperInstance.Update();

            await Task.Delay(16);

            await JSRuntime.InvokeVoidAsync("menuDropdown.startMenuAnimation", _menuRef);
        }

        protected async Task RegisterClickOutside()
        {
            if (_isDisposed || _dotNetRef == null) return;

            try
            {
                await JSRuntime.InvokeVoidAsync(
                    "menuDropdown.registerClickOutside",
                    _triggerRef,
                    _menuRef,
                    _dotNetRef
                );
            }
            catch { }
        }

        protected async Task UnregisterClickOutside()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync(
                    "menuDropdown.unregisterClickOutside",
                    _containerRef
                );
            }
            catch { }
        }

        protected async Task DestroyPopper()
        {
            if (_popperInstance != null)
            {
                try
                {
                    await _popperInstance.Destroy();
                }
                catch (JSException ex)
                {
                    Console.WriteLine($"Error destroying popper: {ex.Message}");
                }
                _popperInstance = null;
            }
        }

        protected async Task ToggleMenu()
        {
            IsOpen = !IsOpen;

            if (IsOpen)
            {
                await JSRuntime.InvokeVoidAsync("menuDropdown.closeOtherMenus", ItemId.ToString(), _dotNetRef);
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("menuDropdown.hideMenu", _menuRef);

                await DestroyPopper();

                await InvokeAsync(StateHasChanged);
            }

            await InvokeAsync(StateHasChanged);
        }

        [JSInvokable]
        public async Task CloseDropdown()
        {
            if (_isDisposed) return;

            IsOpen = false;

            await JSRuntime.InvokeVoidAsync("menuDropdown.hideMenu", _menuRef);

            await DestroyPopper();

            await InvokeAsync(StateHasChanged);
        }

        protected static string GetActionText(MenuAction action) => action switch
        {
            MenuAction.View => "Просмотреть",
            MenuAction.ViewIdea or MenuAction.ViewIdeaMarket => "Открыть идею",
            MenuAction.ViewProfile => "Перейти на профиль",
            MenuAction.ViewLetter => "Просмотреть письмо",
            MenuAction.Edit => "Редактировать",
            MenuAction.TeamRequestAccept => "Принять",
            MenuAction.SetLeader => "Назначить лидером",
            MenuAction.Delete => "Удалить",
            MenuAction.TeamRequestCancel => "Отклонить",
            MenuAction.UnsetLeader => "Снять роль лидера",
            MenuAction.RemoveTeamMember => "Исключить",
            _ => action.ToString()
        };

        protected static string GetActionStyle(MenuAction action) => action switch
        {
            MenuAction.TeamRequestAccept => "text-success",
            MenuAction.SetLeader => "text-primary",
            MenuAction.Delete
            or MenuAction.TeamRequestCancel
            or MenuAction.UnsetLeader
            or MenuAction.RemoveTeamMember => "text-danger",
            _ => string.Empty
        };

        protected async Task HandleActionClick(MenuAction action, object item)
        {
            IsOpen = false;

            await JSRuntime.InvokeVoidAsync("menuDropdown.hideMenu", _menuRef);
            await DestroyPopper();

            await OnAction.InvokeAsync(new() { Action = action, Item = item });
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            try
            {
                if (_popperInstance != null)
                {
                    _ = _popperInstance.Destroy();
                }
                _ = UnregisterClickOutside();
            }
            catch { }

            _dotNetRef?.Dispose();
            _dotNetRef = null;
        }
    }
}
