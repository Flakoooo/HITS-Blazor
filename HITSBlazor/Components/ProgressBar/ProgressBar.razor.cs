using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.ProgressBar
{
    public partial class ProgressBar : IDisposable
    {
        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public double? Score { get; set; }

        [Parameter]
        public string Text { get; set; } = string.Empty;

        [Parameter]
        public int CategoryCounts { get; set; }

        private int _dotCount = 0;
        private Timer? _animationTimer;

        private string _progressStyle = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            StartDotAnimation();
            UpdateProgressBar();
        }

        protected override async Task OnParametersSetAsync() => UpdateProgressBar();

        private void UpdateProgressBar()
        {
            if (Score.HasValue)
            {
                var width = (int)(Score.Value / CategoryCounts * 100);
                var color = width switch
                {
                    <= 60 => "rgb(220, 53, 69)",
                    <= 75 => "rgb(253, 126, 20)",
                    <= 90 => "rgb(255, 193, 7)",
                    >= 91 => "rgb(25, 135, 84)"
                };

                StopAnimation();
                _progressStyle = $"width: {width}%; background-color: {color};";
            }
        }

        private void StartDotAnimation()
        {
            _animationTimer?.Dispose();

            _animationTimer = new Timer(_ =>
            {
                _dotCount = (_dotCount + 1) % 4;
                InvokeAsync(StateHasChanged);
            }, null, 0, 300);
        }

        private void StopAnimation()
        {
            _animationTimer?.Dispose();
            _animationTimer = null;
            _dotCount = 0;
        }

        public void Dispose() => StopAnimation();
    }
}
