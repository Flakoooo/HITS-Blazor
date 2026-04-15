using System.ComponentModel;

namespace HITSBlazor.Components.Modals.RightSideModals.ProfileModal
{
    public enum ProfileModalCategory
    {
        [Description("Общее")]
        General,

        [Description("Компетенции")]
        Skills,

        [Description("Идеи")]
        Ideas,

        [Description("Портфолио")]
        Teams,

        [Description("Тесты")]
        Tests,
    }
}
