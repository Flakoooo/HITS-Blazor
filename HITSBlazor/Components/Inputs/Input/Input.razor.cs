using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Inputs.Input
{
    public partial class Input
    {
        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public string HintText { get; set; } = string.Empty;

        [Parameter]
        public string IconStyle { get; set; } = string.Empty;

        [Parameter]
        public InputType InputType { get; set; } = InputType.Text; 

        [Parameter]
        public string Placeholder { get; set; } = string.Empty;

        [Parameter]
        public int? MaxLength { get; set; }

        [Parameter]
        public bool? IsDisabled { get; set; }

        [Parameter]
        public string? CustomClass { get; set; }

        [Parameter]
        public string Value { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

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

        private Dictionary<string, object> GetInputAttributes()
        {
            var attributes = new Dictionary<string, object>();

            if (MaxLength.HasValue)
            {
                attributes["maxlength"] = MaxLength.Value.ToString();
            }

            if (IsDisabled.HasValue && IsDisabled.Value)
            {
                attributes["disabled"] = "disabled";
            }

            return attributes;
        }

        private async Task OnInputChanged(ChangeEventArgs e)
        {
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(e.Value?.ToString());
        }
    }
}
