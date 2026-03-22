using HITSBlazor.Models.Tests.Entities;

namespace HITSBlazor.Services.TestResults
{
    public interface ITestResultService
    {
        public string BelbinTestName { get; }
        public string TemperTestName { get; }
        public string MindTestName { get; }

        Task<List<TestAllResponse>> GetTestsResultsAsync(
            string? searchText = null,
            HashSet<string>? selectedStudyGroups = null,
            HashSet<string>? selectedBelbinResults = null,
            string? selectedMindResult = null,
            HashSet<string>? selectedExtraversionResults = null,
            HashSet<string>? selectedNeurotismResults = null,
            HashSet<string>? selectedLieResults = null
        );
        Task<TestResult?> GetTestResultAsync(Guid userId, string testName);
    }
}
