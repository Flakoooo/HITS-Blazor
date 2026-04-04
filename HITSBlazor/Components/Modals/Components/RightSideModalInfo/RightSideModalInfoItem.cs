using HITSBlazor.Components.Typography;

namespace HITSBlazor.Components.Modals.Components.RightSideModalInfo
{
    public class RightSideModalInfoItem
    {
        public bool IsInline { get; set; } = false;
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public TextColor? TextColor { get; set; }
        public bool IsLinkable { get; set; } = false;
        public Action? LinkMethod { get; set; } = null;
    }
}
