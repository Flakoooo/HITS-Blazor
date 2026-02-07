using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.TableBodyRow
{
    public partial class TableBodyRow
    {
        [Parameter]
        public bool IsCentered { get; set; } = false;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        private string GetTableRowStyle()
            => IsCentered ? "justify-content-center align-items-center text-center" : string.Empty;
    }
}
