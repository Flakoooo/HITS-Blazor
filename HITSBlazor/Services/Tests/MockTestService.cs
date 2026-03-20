using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Tests.Entities;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Tests;

namespace HITSBlazor.Services.Tests
{
    public class MockTestService : ITestService
    {
        public event Func<Task>? OnTestsStateChanged;
        public event Action? OnTestsStateUpdated;

        private List<Test> _cachedTests = [];
        private DateTime _lastRefreshTime;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

        public string BelbinTestName { get; } = "BelbinTest";
        public string TemperTestName { get; } = "TemperTest";
        public string MindTestName { get; } = "MindTest";

        private async Task RefreshCacheAsync()
        {
            _cachedTests = MockTests.GetAllTests();
            _lastRefreshTime = DateTime.UtcNow;
        }

        public async Task<List<Test>> GetTestsAsync(string? searchText)
        {
            if (_cachedTests.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            var query = _cachedTests.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(t => t.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            return [.. query];
        }

        public async Task<TestResult?> GetTestResultAsync(Guid userId, string testName)
            => MockTestResults.GetTestResult(userId, testName);
    }
}
