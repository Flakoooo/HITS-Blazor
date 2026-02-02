using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HITSBlazor.Components.Collapse
{
    public partial class Collapse
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter]
        public RenderFragment<Collapse> MainContent { get; set; } = default!;

        [Parameter]
        public RenderFragment ExpandedContent { get; set; } = default!;

        [Parameter]
        public string ExpandedClass { get; set; } = string.Empty;

        [Parameter]
        public bool StartCollapsed { get; set; } = true;

        private bool _isExpanded = false;
        private bool _isAnimating = false;
        private double? _height;
        private ElementReference _collapseElement;

        protected override void OnInitialized()
        {
            _isExpanded = !StartCollapsed;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !StartCollapsed)
                await UpdateHeight();
        }

        private string GetCollapseStyle()
        {
            if (_isAnimating && _height.HasValue)
                return $"height: {_height}px; overflow: hidden;";
            else if (_isExpanded && !_isAnimating)
                return "height: auto; overflow: visible;";
            else if (!_isExpanded && !_isAnimating)
                return "height: 0; overflow: hidden;";

            return string.Empty;
        }

        public async Task Toggle()
        {
            if (_isAnimating) return;

            _isAnimating = true;
            if (_isExpanded)
            {
                _height = await GetElementHeight(_collapseElement);
                StateHasChanged();

                await Task.Delay(10);

                _height = 0;
                StateHasChanged();

                await Task.Delay(350);

                _isExpanded = false;
                _isAnimating = false;
                _height = null;
            }
            else
            {
                _isExpanded = true;
                _height = 0;
                StateHasChanged();

                await Task.Delay(10);

                _height = await GetElementHeight(_collapseElement);
                StateHasChanged();

                await Task.Delay(350);

                _isAnimating = false;
                _height = null;
            }

            StateHasChanged();
        }

        private async Task UpdateHeight()
        {
            if (_isExpanded)
            {
                var height = await GetElementHeight(_collapseElement);
                if (height > 0)
                {
                    _height = height;
                    StateHasChanged();
                }
            }
        }

        private async Task<double> GetElementHeight(ElementReference element)
        {
            try
            {
                return await JSRuntime.InvokeAsync<double>("getElementHeight", element);
            }
            catch
            {
                return 0;
            }
        }
    }
}
