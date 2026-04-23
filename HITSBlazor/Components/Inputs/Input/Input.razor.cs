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

        private bool _showError = false;
        private Timer? _debounceTimer;
        private string _pendingValue = string.Empty;

        protected override void OnInitialized()
        {
            if (DebounceDelay > 0)
            {
                _debounceTimer = new Timer(DebounceDelay);
                _debounceTimer.Elapsed += OnDebounceTimerElapsed;
                _debounceTimer.AutoReset = false;
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

            if (DebounceDelay > 0 && ValueChanged.HasDelegate)
            {
                _pendingValue = newValue;

                _debounceTimer?.Stop();

                _debounceTimer?.Start();
            }
            else
            {
                if (ValueChanged.HasDelegate)
                    await ValueChanged.InvokeAsync(newValue);
            }
        }

        private async void OnDebounceTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if (ValueChanged.HasDelegate)
            {
                await InvokeAsync(async () =>
                {
                    await ValueChanged.InvokeAsync(_pendingValue);
                });
            }
        }

        public void Dispose()
        {
            if (_debounceTimer != null)
            {
                _debounceTimer.Stop();
                _debounceTimer.Elapsed -= OnDebounceTimerElapsed;
                _debounceTimer.Dispose();
                _debounceTimer = null;
            }
        }
    }
}
