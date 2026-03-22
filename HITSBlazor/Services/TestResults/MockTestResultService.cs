using HITSBlazor.Components.Inputs.Input;
using HITSBlazor.Models.Tests.Entities;
using HITSBlazor.Utils.Mocks.Tests;
using System.Text.RegularExpressions;

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

        public async Task<List<TestAllResponse>> GetTestsResultsAsync(
            string? searchText,
            HashSet<string>? selectedStudyGroups,
            HashSet<string>? selectedBelbinResults,
            string? selectedMindResult,
            HashSet<string>? selectedExtraversionResults,
            HashSet<string>? selectedNeurotismResults,
            HashSet<string>? selectedLieResults
        )
        {
            if (_cachedTestsResults.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            var query = _cachedTestsResults.AsEnumerable();

            if (selectedStudyGroups?.Count > 0)
                query = query.Where(t => selectedStudyGroups.Contains(t.User.StudyGroup.ToLower()));

            if (selectedBelbinResults?.Count > 0)
                query = query.Where(t => selectedBelbinResults.Contains(t.BelbinResult.ToLower()));

            if (!string.IsNullOrWhiteSpace(selectedMindResult))
            {
                query = query.Where(t =>
                {
                    if (string.IsNullOrEmpty(t.MindResult)) return false;

                    var pattern = @"([А-Яа-я]+ский стиль): \((\d+)\)";
                    var matches = Regex.Matches(t.MindResult, pattern);

                    var scores = matches
                        .Select(m => new
                        {
                            Style = m.Groups[1].Value,
                            Score = int.Parse(m.Groups[2].Value)
                        })
                        .ToList();

                    var selected = scores.FirstOrDefault(s => s.Style.Equals(selectedMindResult, StringComparison.CurrentCultureIgnoreCase));
                    if (selected == null) return false;

                    int maxScore = scores.Max(s => s.Score);
                    return selected.Score == maxScore;
                });
            }

            if (selectedExtraversionResults?.Count > 0 || selectedNeurotismResults?.Count > 0 || selectedLieResults?.Count > 0)
            {
                query = query.Where(t =>
                {
                    if (string.IsNullOrEmpty(t.TemperResult)) return false;

                    var pattern = @"([^:]+): \((\d+)\) (.+?)(?=\n|$)";
                    var matches = Regex.Matches(t.TemperResult, pattern, RegexOptions.Multiline);

                    var traits = new Dictionary<string, string>();
                    foreach (Match match in matches)
                    {
                        string name = match.Groups[1].Value.Trim();
                        string level = match.Groups[3].Value.Trim();

                        Console.WriteLine(name);
                        Console.WriteLine(level);

                        if (name.Contains("Экстраверсии"))
                            traits["Extraversion"] = level;
                        else if (name.Contains("Нейротизма"))
                            traits["Neurotism"] = level;
                        else if (name.Contains("Лжи"))
                            traits["Lie"] = level;
                    }

                    bool extraversionOk = selectedExtraversionResults is null ||
                                          selectedExtraversionResults.Count == 0 ||
                                          (traits.TryGetValue("Extraversion", out var extraversion) &&
                                           selectedExtraversionResults.Contains(extraversion.ToLower()));

                    bool neurotismOk = selectedNeurotismResults is null ||
                                       selectedNeurotismResults.Count == 0 ||
                                       (traits.TryGetValue("Neurotism", out var neurotism) &&
                                        selectedNeurotismResults.Contains(neurotism.ToLower()));

                    bool lieOk = selectedLieResults is null ||
                                 selectedLieResults.Count == 0 ||
                                 (traits.TryGetValue("Lie", out var lie) &&
                                  selectedLieResults.Contains(lie.ToLower()));

                    return extraversionOk && neurotismOk && lieOk;
                });
            }

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(t => 
                    t.User.FullName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) ||
                    t.BelbinResult.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) ||
                    t.TemperResult.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) ||
                    t.MindResult.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)
                );

            return [.. query];
        }

        public async Task<List<TestResult>> GetTestResultsAsync(string testName) => MockTestResults.GetTestResults(testName);

        public async Task<TestResult?> GetTestResultAsync(Guid userId, string testName)
            => MockTestResults.GetTestResult(userId, testName);
    }
}
