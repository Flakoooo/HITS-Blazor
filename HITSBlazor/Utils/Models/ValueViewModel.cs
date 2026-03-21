namespace HITSBlazor.Utils.Models
{
    public class ValueViewModel<T>(T value, string displayText) : ViewModelBase
    {
        public T Value { get; private set; } = value;
        public string DisplayText { get; set; } = displayText;

        public override string GetDisplayInfo() => DisplayText;

        public override object GetId() => Value ?? new object();
    }
}
