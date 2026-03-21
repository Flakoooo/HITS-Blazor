using HITSBlazor.Models.Tests.Entities;
using HITSBlazor.Utils.Mocks.Tests;

namespace HITSBlazor.Services.TestResults
{
    public class MockTestResultService : ITestResultService
    {
        private List<TestAllResponse> _cachedTestsResults = [];
        private DateTime _lastRefreshTime;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

        public string BelbinTestName { get; } = "BelbinTest";
        public string TemperTestName { get; } = "TemperTest";
        public string MindTestName { get; } = "MindTest";

        private async Task RefreshCacheAsync()
        {
            _cachedTestsResults = MockTestResults.GetAllTestsResults();
            _lastRefreshTime = DateTime.UtcNow;
        }

        public async Task<List<TestAllResponse>> GetTestsResultsAsync(string? searchText)
        {
            if (_cachedTestsResults.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            var query = _cachedTestsResults.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(t => 
                    t.User.FullName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) ||
                    t.BelbinResult.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) ||
                    t.TemperResult.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) ||
                    t.MindResult.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)
                );

            return [.. query];
        }

        public async Task<TestResult?> GetTestResultAsync(Guid userId, string testName)
            => MockTestResults.GetTestResult(userId, testName);
    }
}
