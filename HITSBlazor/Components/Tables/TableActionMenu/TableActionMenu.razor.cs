using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HITSBlazor.Components.Tables.TableActionMenu
{
    public partial class TableActionMenu : IDisposable
    {
        [Parameter]
        public Guid ItemId { get; set; }

        [Parameter]
        public Dictionary<TableAction, object> ActionIds { get; set; } = [];

        [Parameter]
        public EventCallback<TableActionContext> OnAction { get; set; }

        [Parameter]
        public bool ShowToRight { get; set; } = false;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        private ElementReference _containerRef;
        private ElementReference _triggerRef;
        private DotNetObjectReference<TableActionMenu>? _dotNetRef;
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
                    "menuDropdown.shouldShowAbove",
                    new
                    {
                        triggerElement = _triggerRef,
                        minHeight = 18 + (32 * ActionIds.Count) + ActionIds.Count - 1 + 10
                    }
                );

                var styleBuilder = new System.Text.StringBuilder();

                if (shouldShowAbove)
                {
                    styleBuilder.Append("bottom: 100%; ");
                    styleBuilder.Append(ShowToRight ? "right: 0; " : "left: 0; ");
                    styleBuilder.Append("margin-bottom: 5px; ");
                }
                else
                {
                    styleBuilder.Append("top: 100%; ");
                    styleBuilder.Append(ShowToRight ? "right: 0; " : "left: 0; ");
                    styleBuilder.Append("margin-top: 5px; ");
                }

                _menuStyle = styleBuilder.ToString();
                StateHasChanged();
            }
            catch (Exception)
            {
                _menuStyle = "top: 100%; left: 0; margin-top: 5px;";
            }
        }

        private async Task RegisterClickOutside()
        {
            if (_isDisposed || _dotNetRef == null) return;

            try
            {
                await JSRuntime.InvokeVoidAsync(
                    "menuDropdown.registerClickOutside",
                    _containerRef,
                    _dotNetRef,
                    ItemId.ToString()
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

        private async Task ToggleMenu()
        {
            IsOpen = !IsOpen;

            if (IsOpen)
                await JSRuntime.InvokeVoidAsync("menuDropdown.closeOtherMenus", ItemId.ToString());

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
            _menuStyle = "";

            await OnAction.InvokeAsync(new() { Action = action, Item = item });
        }

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
