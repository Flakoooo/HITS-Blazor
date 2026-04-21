using HITSBlazor.Utils.Attributes;
using System.ComponentModel;

namespace HITSBlazor.Components.Placeholders.Placeholder
{
    public enum PlaceholderHeight
    {
        [Description("")]
        [Style("")]
        None,

        [Description("smaller")]
        [Style("height: 40px;")]
        Smaller,

        [Description("small")]
        [Style("height: 50px;")]
        Small,

        [Description("regular")]
        [Style("height: 70px;")]
        Regular,

        [Description("medium")]
        [Style("height: 100px;")]
        Medium,

        [Description("second")]
        [Style("height: 200px;")]
        Second
    }
}
