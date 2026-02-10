using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Inputs.SearchInput
{
    public partial class SearchInput
    {
        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        private async Task OnInputChanged(ChangeEventArgs e)
        {
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(e.Value?.ToString());
        }
    }
}
