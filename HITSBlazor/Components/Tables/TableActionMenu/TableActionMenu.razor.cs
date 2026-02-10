using KristofferStrube.Blazor.Popper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HITSBlazor.Components.Tables.TableActionMenu
{
    public partial class TableActionMenu : IDisposable
    {
        [Parameter]
        public Guid ItemId { get; set; }

        [Parameter]
        public Strategy StrategyPosition { get; set; } = Strategy.Absolute;

        [Parameter]
        public Dictionary<TableAction, object> ActionIds { get; set; } = [];

        [Parameter]
        public EventCallback<TableActionContext> OnAction { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        [Inject]
        private Popper PopperService { get; set; } = null!;

        private ElementReference _containerRef;
        private ElementReference _triggerRef;
        private ElementReference _menuRef;
        private DotNetObjectReference<TableActionMenu>? _dotNetRef;
        private Instance? _popperInstance;

        private static List<Modifier> InstanceModifiers { get; } =
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

        private bool IsOpen { get; set; }
        private bool _isDisposed;

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

        private async Task CreateOrUpdatePopper()
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

        private async Task RegisterClickOutside()
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

        private async Task UnregisterClickOutside()
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

        private async Task DestroyPopper()
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

        private async Task ToggleMenu()
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

        private static string GetActionText(TableAction action) => action switch
        {
            TableAction.View => "Просмотреть",
            TableAction.ViewIdea or TableAction.ViewIdeaMarket => "Открыть идею",
            TableAction.ViewProfile => "Перейти на профиль",
            TableAction.ViewLetter => "Просмотреть письмо",
            TableAction.Edit => "Редактировать",
            TableAction.TeamRequestAccept => "Принять",
            TableAction.SetLeader => "Назначить лидером",
            TableAction.Delete => "Удалить",
            TableAction.TeamRequestCancel => "Отклонить",
            TableAction.UnsetLeader => "Снять роль лидера",
            TableAction.RemoveTeamMember => "Исключить",
                _ => action.ToString()
        };

        private static string GetActionStyle(TableAction action) => action switch
        {
            TableAction.TeamRequestAccept => "text-success",
            TableAction.SetLeader => "text-primary",
            TableAction.Delete 
            or TableAction.TeamRequestCancel 
            or TableAction.UnsetLeader
            or TableAction.RemoveTeamMember => "text-danger",
            _ => string.Empty
        };

        private async Task HandleActionClick(TableAction action, object item)
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
