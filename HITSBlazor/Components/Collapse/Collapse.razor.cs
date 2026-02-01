using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Collapse
{
    public partial class Collapse
    {
        [Parameter]
        public RenderFragment<Collapse> MainContent { get; set; } = default!;

        [Parameter]
        public RenderFragment ExpandedContent { get; set; } = default!;

        [Parameter]
        public string ExpandedClass { get; set; } = string.Empty;

        [Parameter]
        public bool StartCollapsed { get; set; } = true;

        private bool _isExpanded = false;

        protected override void OnInitialized()
        {
            _isExpanded = !StartCollapsed;
        }

        public void Toggle()
        {
            _isExpanded = !_isExpanded;
            StateHasChanged();
        }
    }
}
