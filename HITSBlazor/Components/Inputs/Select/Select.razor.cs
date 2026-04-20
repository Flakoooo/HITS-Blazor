using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Inputs.Select
{
    public partial class Select
    {
        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public string Label { get; set; } = string.Empty;

        [Parameter]
        public string Value { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public List<KeyValuePair<int, string>> Options { get; set; } = [];

        [Parameter]
        public bool NeedValidation { get; set; } = false;

        [Parameter]
        public string? ErrorMessage { get; set; } = "Поле не заполнено";

        private bool _showError = false;

        protected override void OnParametersSet()
        {
            _showError = NeedValidation && string.IsNullOrWhiteSpace(Value);
            StateHasChanged();
        }

        private async Task OnOptionChanged(ChangeEventArgs e)
            => await ValueChanged.InvokeAsync(e.Value?.ToString() ?? "");
    }
}
