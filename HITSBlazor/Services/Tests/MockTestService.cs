using HITSBlazor.Models.Tests.Entities;
using HITSBlazor.Utils.Mocks.Tests;

namespace HITSBlazor.Services.Tests
{
    public class MockTestService : ITestService
    {
        public string BelbinTestName { get; } = "BelbinTest";
        public string TemperTestName { get; } = "TemperTest";
        public string MindTestName { get; } = "MindTest";

        public async Task<TestResult?> GetTestResultAsync(Guid userId, string testName)
            => MockTestResults.GetTestResult(userId, testName);
    }
}
