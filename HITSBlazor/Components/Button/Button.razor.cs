using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Button
{
    public partial class Button
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string? Text { get; set; }
        [Parameter] public string? Type { get; set; } = "button";
        [Parameter] public string? Variant { get; set; } = "primary";
        [Parameter] public string? ClassName { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool IsLoading { get; set; }
        [Parameter] public string? PrependIconName { get; set; }
        [Parameter] public string? AppendIconName { get; set; }
        [Parameter] public bool MaxWidth { get; set; }
        [Parameter] public EventCallback OnClick { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> AdditionalAttributes { get; set; } = [];

        private bool IsDisabled => Disabled || IsLoading;

        private string GetButtonClass()
        {
            var classes = new List<string> { "btn d-flex" };

            if (!string.IsNullOrEmpty(Variant))
                classes.Add($"btn-{Variant}");

            if (MaxWidth)
                classes.Add("btn-maxWidth");

            if (!string.IsNullOrEmpty(ClassName))
                classes.Add(ClassName);

            return string.Join(" ", classes);
        }

        private async Task OnClickHandler()
        {
            if (!Disabled && !IsLoading && OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync();
            }
        }
    }
}
