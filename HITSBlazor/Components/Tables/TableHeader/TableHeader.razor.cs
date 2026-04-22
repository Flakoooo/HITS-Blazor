using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.TableHeader
{
    public partial class TableHeader
    {
        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public List<TableHeaderItem> Items { get; set; } = [];

        [Parameter]
        public bool TableActionMenuIsEnable { get; set; } = false;

        [Parameter]
        public EventCallback<(string?, bool?)> OrderChanged { get; set; }

        private static string GetSortIcon(TableHeaderItem item)
        {
            if (item.IsOrdered is null)
                return "bi-arrow-down-up";
            else if (item.IsOrdered.Value)
                return "bi-chevron-down";
            else
                return "bi-chevron-up";
        }

        private static string GetItemClass(bool isCentered)
            => isCentered ? "justify-content-center align-items-center text-center" : string.Empty;

        private void ResetHeadersStatus(string? orderName)
        {
            foreach (var i in Items)
                if (i.OrderBy != orderName)
                    i.IsOrdered = null;
        }

        private async Task SetOrderState(TableHeaderItem item)
        {
            if (item.IsOrdered is null)
            {
                item.IsOrdered = true;
                ResetHeadersStatus(item.OrderBy);
            }    
            else if (item.IsOrdered.Value)
            {
                item.IsOrdered = false;
                ResetHeadersStatus(item.OrderBy);
            }
            else
                item.IsOrdered = null;

            await OrderChanged.InvokeAsync((item.OrderBy, item.IsOrdered));
            StateHasChanged();
        }
    }
}
