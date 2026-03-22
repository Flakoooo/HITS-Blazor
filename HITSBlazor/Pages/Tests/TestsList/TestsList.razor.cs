using HITSBlazor.Components.Modals.RightSideModals.ProfileModal;
using HITSBlazor.Components.Modals.RightSideModals.TestModal;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Tests.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Modal;
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

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private bool _isLoading = true;

        private string? _searchText = null;
        private List<Test> _tests = [];

        private readonly List<TableHeaderItem> _testTableHeader = [ new() { Text = "Название" } ];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            await LoadTestsAsync();

            _isLoading = false;
        }

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

        private void ShowTestModal(Test test)
        {
            ModalService.Show<TestModal>(
                ModalType.RightSide,
                parameters: new Dictionary<string, object> { [nameof(TestModal.CurrentTest)] = test }
            );
        }
    }
}
