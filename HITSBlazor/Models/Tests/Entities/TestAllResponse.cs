using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Models.Tests.Entities
{
    public class TestAllResponse
    {
        public User User { get; set; } = new();
        public string BelbinResult { get; set; } = string.Empty;
        public string TemperResult { get; set; } = string.Empty;
        public string MindResult { get; set; } = string.Empty;
    }
}
