using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Tests.Entities;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.TestResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Tests.CurrentTestsResults
{
    [Authorize]
    [Route("tests/{TestName}/result/all")]
    public partial class CurrentTestsResults
    {
        [Inject]
        private ITestResultService TestResultService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public string TestName { get; set; } = string.Empty;

        private bool _isLoading = true;

        private string _searchText = string.Empty;

        private readonly List<TableHeaderItem> _resultTableHeader =
        [
            new() { Text = "Почта"      },
            new() { Text = "Имя"        },
            new() { Text = "Фамилия"    },
            new() { Text = "Группа"     },
            new() { Text = "Результат"  }
        ];

        private List<TestResult> _results = [];
        private List<TestResult> _searchedResults = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (!string.IsNullOrWhiteSpace(TestName))
                _results = await TestResultService.GetTestResultsAsync(TestName);

            _searchedResults = [.. _results];

            _isLoading = false;
        }

        private async Task SearchResult(string value)
        {
            _searchText = value;
            _searchedResults = [.. _results.Where(tr => 
                tr.User.FullName.Contains(_searchText, StringComparison.CurrentCultureIgnoreCase) ||
                tr.TestResultValue.Contains(_searchText, StringComparison.CurrentCultureIgnoreCase))
            ];
        }
    }
}
