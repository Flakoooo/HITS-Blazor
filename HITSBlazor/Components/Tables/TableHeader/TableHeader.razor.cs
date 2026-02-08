using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.TableHeader
{
    public partial class TableHeader
    {
        [Parameter]
        public IReadOnlyList<TableHeaderItem> Items { get; set; } = [];

        [Parameter]
        public bool TableActionMenuIsEnable { get; set; } = false;

        private static string GetItemClass(bool isCentered)
            => isCentered ? "justify-content-center align-items-center text-center" : string.Empty;
    }
}
