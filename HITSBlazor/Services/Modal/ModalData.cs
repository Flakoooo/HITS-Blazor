namespace HITSBlazor.Services.Modal
{
    public class ModalData
    {
        public Type? ComponentType { get; set; }
        public bool BlockCloseModal { get; set; } = false;
        public Dictionary<string, object> Parameters { get; set; } = [];
    }
}
