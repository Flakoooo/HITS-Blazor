namespace HITSBlazor.Components.Tables.TableHeader
{
    public class TableHeaderItem
    {
        public string Text { get; set; } = string.Empty;
        public bool InCentered { get; set; } = false;
        public string ColumnClass { get; set; } = string.Empty;
        public string? OrderBy { get; set; }
        public bool? IsOrdered { get; set; }
    }
}
