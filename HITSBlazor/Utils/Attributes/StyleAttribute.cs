namespace HITSBlazor.Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class StyleAttribute(string value) : Attribute
    {
        public string Style { get; } = value;
    }
}
