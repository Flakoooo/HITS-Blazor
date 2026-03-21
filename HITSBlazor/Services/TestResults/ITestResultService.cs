using HITSBlazor.Models.Tests.Entities;

namespace HITSBlazor.Services.TestResults
{
    public interface ITestResultService
    {
        public string BelbinTestName { get; }
        public string TemperTestName { get; }
        public string MindTestName { get; }

        Task<List<TestAllResponse>> GetTestsResultsAsync(string? searchText = null);
        Task<TestResult?> GetTestResultAsync(Guid userId, string testName);
    }
}
