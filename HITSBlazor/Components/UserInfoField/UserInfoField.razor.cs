using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.UserInfoField
{
    public partial class UserInfoField
    {
        [Parameter]
        public string Title { get; set; } = string.Empty;

        [Parameter]
        public string Value { get; set; } = string.Empty;

        [Parameter]
        public bool IsEditing { get; set; } = false;

        [Parameter]
        public bool IsCurrentUser { get; set; } = false;

        [Parameter] 
        public EventCallback OnEdit { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        private bool _inputDisable = true;
        private ElementReference _inputRef;
        private bool _previousIsEditing;

        protected override void OnParametersSet()
        {
            _previousIsEditing = IsEditing;

            _inputDisable = !IsEditing;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (_previousIsEditing && !IsEditing)
            {
                await InvokeAsync(StateHasChanged);
            }

            if (IsEditing && !_previousIsEditing)
            {
                await Task.Delay(50);
                await _inputRef.FocusAsync();
            }
        }

        private async Task StartEdit()
        {
            await OnEdit.InvokeAsync();
        }

        private async Task OnInputChanged(ChangeEventArgs e)
        {
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(e.Value?.ToString());
        }
    }
}
