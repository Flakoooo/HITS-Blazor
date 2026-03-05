using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Inputs.Number
{
    public partial class Number
    {
        [Parameter]
        public string MinValue { get; set; } = string.Empty;

        [Parameter]
        public string MaxValue { get; set; } = string.Empty;

        [Parameter]
        public string Placeholder { get; set; } = string.Empty;

        [Parameter]
        public int Value { get; set; }

        [Parameter]
        public EventCallback<int> ValueChanged { get; set; }

        [Parameter]
        public bool NeedValidation { get; set; } = false;

        [Parameter]
        public string? ErrorMessage { get; set; } = string.Empty;

        private bool _showError = false;

        protected override void OnParametersSet()
        {
            _showError = NeedValidation && (Value > 30 || Value < 2);
            ErrorMessage = $"Значение должно быть от {MinValue} до {MaxValue}";
            StateHasChanged();
        }

        private async Task OnInputChanged(ChangeEventArgs e)
        {
            if (ValueChanged.HasDelegate && int.TryParse(e.Value?.ToString(), out int value))
                await ValueChanged.InvokeAsync(value);
        }
    }
}
