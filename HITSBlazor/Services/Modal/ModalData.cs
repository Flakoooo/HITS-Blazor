namespace HITSBlazor.Services.Modal
{
    public class ModalData
    {
        public Type? ComponentType { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = [];
    }
}
