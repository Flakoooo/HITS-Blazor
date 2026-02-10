namespace HITSBlazor.Components.Modals.RightSideModals.IdeaModal
{
    public class IdeaModalItem
    {
        public string Title { get; set; } = string.Empty;
        public object? Data { get; set; }
        public bool IsExpanded { get; set; } = true;
    }
}
