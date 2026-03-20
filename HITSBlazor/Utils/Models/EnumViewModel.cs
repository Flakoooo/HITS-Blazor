using HITSBlazor.Utils.Attributes;
using System.ComponentModel;
using System.Reflection;

namespace HITSBlazor.Utils.Models
{
    public class EnumViewModel<T>(T value) : ViewModelBase where T : Enum
    {
        public T Value { get; private set; } = value;

        public static string GetTranslation(Enum? value)
        {
            if (value is null) return nameof(value);

            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

            return attribute?.Description ?? value.ToString();
        }

        public static string GetStyle(Enum? value)
        {
            if (value is null) return nameof(value);

            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<StyleAttribute>();

            return attribute?.Style ?? value.ToString();
        }

        public override string GetDisplayInfo() => GetTranslation(Value);

        public override object GetId() => Value;
    }
}
