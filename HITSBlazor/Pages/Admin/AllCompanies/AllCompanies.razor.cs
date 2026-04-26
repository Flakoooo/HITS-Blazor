using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.CenterModals.CompanyModal;
using HITSBlazor.Components.Tables.TableComponent;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Services.Companies;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Admin.AllCompanies
{
    [Authorize]
    [Route("/admin/companies")]
    public partial class AllCompanies
    {
        [Inject]
        private ICompanyService CompanyService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private bool _isLoading = true;

        private TableComponent? _tableComponent;

        private string _searchText = string.Empty;

        private readonly List<Company> _companies = [];

        private static List<TableHeaderItem> TableHeaderItems => 
        [
            new TableHeaderItem { Text = "Название",        ColumnClass="col-5" },
            new TableHeaderItem { Text = "Руководитель",    ColumnClass="col-7" }
        ];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            CompanyService.OnCompanyHasCreated += CompanyHasCreated;
            CompanyService.OnCompanyHasUpdated += CompanyHasUpdated;
            CompanyService.OnCompanyHasDeleted += CompanyHasDeleted;

            await LoadCompaniesAsync();

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async Task AdditionalAfterRenderMethod()
        {
            if (_tableComponent != null)
                _tableContainer = _tableComponent.ScrollContainer;
        }

        protected override int GetCurrentItemsCount() => _companies.Count;

        protected override async Task OnLoadMoreItemsAsync()
        {
            await LoadCompaniesAsync(append: true);
        }

        private async Task LoadCompaniesAsync(bool append = false) => await LoadDataAsync(
            _companies,
            () => CompanyService.GetCompaniesAsync(
                _currentPage,
                searchText: _searchText
            ),
            append: append
        );

        private async Task SeacrhCompany(string value)
        {
            _searchText = value;
            ResetPagination();
            await LoadCompaniesAsync();
        }

        private void ShowCompanyModal(Guid? guid = null) => ModalService.Show<CompanyModal>(
            ModalType.Center,
            parameters: guid.HasValue
                ? new Dictionary<string, object> { [nameof(CompanyModal.CompanyId)] = guid.Value }
                : null
        );

        private async Task OnCompanyAction(TableActionContext context)
        {
            if (context.Action == MenuAction.Edit && context.Item is Guid guid)
            {
                ShowCompanyModal(guid);
            }
            else if (context.Action == MenuAction.Delete)
            {
                if (context.Item is not Company company) return;

                ModalService.ShowConfirmModal(
                    $"Вы действительно хотите удалить {company.Name}?",
                    () => CompanyService.DeleteCompanyAsync(company),
                    confirmButtonVariant: ButtonVariant.Danger,
                    confirmButtonText: "Удалить"
                );
            }
        }

        private void CompanyHasCreated(Company newCompany)
        {
            _companies.Add(newCompany);
            ++_totalCount;
            StateHasChanged();
        }

        private void CompanyHasUpdated(Company updatedCompany)
        {
            var company = _companies.FirstOrDefault(c => c.Id == updatedCompany.Id);
            if (company is null) return;

            company.Name = updatedCompany.Name;
            company.Owner = updatedCompany.Owner;
            company.Members = updatedCompany.Members;

            StateHasChanged();
        }

        private void CompanyHasDeleted(Company company)
        {
            if (_companies.Remove(company))
            {
                --_totalCount;
                StateHasChanged();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            CompanyService.OnCompanyHasCreated -= CompanyHasCreated;
            CompanyService.OnCompanyHasUpdated -= CompanyHasUpdated;
            CompanyService.OnCompanyHasDeleted -= CompanyHasDeleted;

            await ValueTask.CompletedTask;
        }
    }
}
