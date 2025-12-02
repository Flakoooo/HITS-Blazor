using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Models.Tests.Entities
{
    public class TestResult
    {
        public string Id { get; set; } = string.Empty;
        public User User { get; set; } = new();
        public string TestName { get; set; } = string.Empty;
        public string TestResultValue { get; set; } = string.Empty;
    }
}
