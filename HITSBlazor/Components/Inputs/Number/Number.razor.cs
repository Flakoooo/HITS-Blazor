using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Inputs.Number
{
    public partial class Number
    {
        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public int? MinValue { get; set; }

        [Parameter]
        public int? MaxValue { get; set; }

        [Parameter]
        public string Placeholder { get; set; } = string.Empty;

        [Parameter]
        public int? Value { get; set; }

        [Parameter]
        public EventCallback<int?> ValueChanged { get; set; }

        [Parameter]
        public bool NeedValidation { get; set; } = false;

        [Parameter]
        public string? ErrorMessage { get; set; } = string.Empty;

        private bool _showError = false;

        protected override void OnInitialized()
        {
            if (Value < MinValue || Value > MaxValue)
                Value = null;
        }

        protected override void OnParametersSet()
        {
            _showError = NeedValidation && (!Value.HasValue || Value.Value > MaxValue || Value.Value < MinValue);

            ErrorMessage = $"Значение должно быть от {MinValue} до {MaxValue}";
            StateHasChanged();
        }

        private async Task OnInputChanged(int? value)
        {
            if (value < MinValue) value = MinValue.Value;
            else if (value > MaxValue) value = MaxValue.Value;

            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(value);
        }
    }
}
