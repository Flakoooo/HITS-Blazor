using HITSBlazor.Components.Tables.TableHeader;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.TableComponent
{
    public partial class TableComponent
    {
        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public bool IsDataLoading { get; set; } = false;

        [Parameter]
        public ICollection<TableHeaderItem> HeaderItems { get; set; } = [];

        [Parameter]
        public Func<string?, bool?, Task>? OrderMethod { get; set; }

        [Parameter]
        public bool TableActionMenuIsEnable { get; set; } = false;

        [Parameter]
        public RenderFragment? TableContent { get; set; }

        [Parameter]
        public RenderFragment? FilterContent { get; set; }

        [Parameter]
        public string MaxHeight { get; set; } = "75vh";

        public ElementReference ScrollContainer => _tableScrollContainer;

        private ElementReference _tableScrollContainer;

        private async Task ExecuteOrder(string? value1, bool? value2)
        {
            if (OrderMethod is null) return;

            await OrderMethod(value1, value2);
        }
    }
}
