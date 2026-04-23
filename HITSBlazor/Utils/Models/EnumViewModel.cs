using HITSBlazor.Utils.EnumUIConverters;

namespace HITSBlazor.Utils.Models
{
    public class EnumViewModel<T>(T value) : ViewModelBase where T : Enum
    {
        public T Value { get; private set; } = value;

        public EnumUIResult GetInfo() => EnumUIConverter.GetInfo(Value);

        public string GetTranslation() => GetInfo().DisplayText;

        public string GetStyle() => GetInfo().DisplayStyle;

        public override string GetDisplayInfo() => GetTranslation();

        public override object GetId() => Value;
    }
}
