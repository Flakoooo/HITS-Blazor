using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.TableComponent
{
    public abstract class TableRowDefinition<T>
    {
        public abstract int ColumnCount { get; }

        public abstract RenderFragment RenderCell(T item, int columnIndex);

        public abstract string GetHeaderTitle(int columnIndex);
        public abstract bool IsColumnCentered(int columnIndex);
    }
}
