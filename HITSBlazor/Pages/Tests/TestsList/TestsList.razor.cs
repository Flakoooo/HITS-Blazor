using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Tests.Entities;
using HITSBlazor.Services.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Tests.TestsList
{
    [Authorize]
    [Route("tests/list")]
    public partial class TestsList
    {
        [Inject]
        private ITestService TestService { get; set; } = null!;

        private bool _isLoading = true;

        private string? _searchText = null;
        private List<Test> _tests = [];

        private readonly List<TableHeaderItem> _testTableHeader = [ new() { Text = "Название" } ];

        private async Task LoadTestsAsync()
        {
            _tests = await TestService.GetTestsAsync(
                    searchText: _searchText
            );
            StateHasChanged();
        }

        private async Task SearchTest(string value)
        {
            _searchText = value;
            await LoadTestsAsync();
        }
    }
}
