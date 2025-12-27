namespace HITSBlazor.Components.SwitchTabs
{
    public class SwitchTab
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public Action? Click { get; set; }
    }

}
