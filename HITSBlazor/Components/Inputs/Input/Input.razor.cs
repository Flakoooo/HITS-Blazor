using HITSBlazor.Services.Debounce;
using HITSBlazor.Utils.Validation;
using Microsoft.AspNetCore.Components;
using System.Timers;
using Timer = System.Timers.Timer;

namespace HITSBlazor.Components.Inputs.Input
{
    public partial class Input : IDisposable
    {
        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public int Width { get; set; } = 100;

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
        public Func<string, ValidationEvaluation>? CustomValidation { get; set; }

        [Parameter]
        public string? ErrorMessage { get; set; } = "Поле не заполнено";

        [Parameter]
        public int DebounceDelay { get; set; } = 0;

        private DebounceHelper? _searchDebounce;

        private bool _showError = false;
        private string _pendingValue = string.Empty;

        protected override void OnInitialized()
        {
            if (ValueChanged.HasDelegate && DebounceDelay > 0)
            {
                _searchDebounce = new DebounceHelper(DebounceDelay, async () =>
                {
                    await InvokeAsync(async () =>
                    {
                        await ValueChanged.InvokeAsync(_pendingValue);
                    });
                });
            }
        }

        protected override void OnParametersSet()
        {
            if (NeedValidation)
            {
                if (CustomValidation is not null)
                {
                    var result = CustomValidation(Value);
                    ErrorMessage = result.Message;
                    _showError = !result.IsValid;
                }
                else
                {
                    _showError = string.IsNullOrWhiteSpace(Value);
                }
            }
            else _showError = false;

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
            var newValue = e.Value?.ToString() ?? string.Empty;

            if (ValueChanged.HasDelegate)
            {
                if (DebounceDelay > 0)
                {
                    _pendingValue = newValue;
                    _searchDebounce?.Trigger();
                }
                else
                {
                    await ValueChanged.InvokeAsync(newValue);
                }
            }
        }

        public void Dispose()
        {
            _searchDebounce?.Dispose();
        }
    }
}
