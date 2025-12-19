namespace HITSBlazor.Utils
{
    public class AppSettings
    {
        public string Environment { get; set; } = "Development";
        public bool UseMockData { get; set; } = true;
        public string ApiBaseUrl { get; set; } = "http://localhost:8080/api";
    }
}
