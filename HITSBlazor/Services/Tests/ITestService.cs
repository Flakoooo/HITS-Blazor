using HITSBlazor.Models.Tests.Entities;

namespace HITSBlazor.Services.Tests
{
    public interface ITestService
    {
        public string BelbinTestName { get; }
        public string TemperTestName { get; }
        public string MindTestName { get; }

        Task<TestResult?> GetTestResultAsync(Guid userId, string testName);
    }
}
