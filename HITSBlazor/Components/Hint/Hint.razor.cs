using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HITSBlazor.Components.Hint
{
    public partial class Hint
    {
        [Parameter]
        public string Text { get; set; } = string.Empty;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        private class DomRect
        {
            public float Left { get; set; }
            public float Top { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public float Right { get; set; }
            public float Bottom { get; set; }
        }

        private ElementReference _containerRef;
        private ElementReference _tooltipRef;
        private string _placement = "top";
        private bool _isVisible;

        private async Task ShowTooltip()
        {
            if (_isVisible) return;

            _isVisible = true;
            _placement = "top";
            StateHasChanged();

            await Task.Delay(16);
            await CalculateBestPlacement();

            StateHasChanged();
        }

        private async Task HideTooltip()
        {
            _isVisible = false;
            StateHasChanged();
        }

        private async Task CalculateBestPlacement()
        {
            try
            {
                var rect = await JSRuntime.InvokeAsync<DomRect>("hintHelper.getBoundingClientRect", _containerRef);
                var tooltipRect = await JSRuntime.InvokeAsync<DomRect>("hintHelper.getBoundingClientRect", _tooltipRef);

                var windowHeight = await JSRuntime.InvokeAsync<double>("hintHelper.getWindowHeight");
                var windowWidth = await JSRuntime.InvokeAsync<double>("hintHelper.getWindowWidth");

                if (rect.Top > tooltipRect.Height + 10)
                {
                    _placement = "top";
                    return;
                }

                if ((windowWidth - rect.Right) > tooltipRect.Width + 10)
                {
                    double tooltipTop = rect.Top + (rect.Height / 2) - (tooltipRect.Height / 2);

                    if (tooltipTop > 0 && (tooltipTop + tooltipRect.Height) < windowHeight)
                    {
                        _placement = "right";
                        return;
                    }
                }

                if ((windowHeight - rect.Bottom) > tooltipRect.Height + 10)
                {
                    _placement = "bottom";
                    return;
                }

                _placement = "top";
            }
            catch (Exception)
            {
                _placement = "top";
            }
        }
    }
}
