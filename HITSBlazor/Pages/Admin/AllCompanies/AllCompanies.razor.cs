using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.CenterModals.CompanyModal;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Companies;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;

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

        private string _searchText = string.Empty;

        private List<Company> _companies = [];

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

        protected override int GetCurrentItemsCount() => _companies.Count;

        protected override async Task OnLoadMoreItemsAsync()
        {
            await LoadCompaniesAsync(append: true);
        }

        private async Task LoadCompaniesAsync(bool append = false)
        {
            Console.WriteLine("чекаем работу");
            if (!append)
            {
                ResetPagination();
                _companies.Clear();
            }

            StateHasChanged();

            var listResponse = await CompanyService.GetCompaniesAsync(
                _currentPage,
                searchText: _searchText
            );

            Console.WriteLine($"page {_currentPage}");
            _totalCount = listResponse.Count;
            if (listResponse.List.Count > 0)
            {
                if (append)
                    _companies.AddRange(listResponse.List);
                else
                {
                    _companies.Clear();
                    _companies.AddRange(listResponse.List);
                }

                IncrementPage();
            }

            StateHasChanged();
        }

        private async Task SeacrhCompany(string value)
        {
            _searchText = value;
            ResetPagination();
            await LoadCompaniesAsync();
        }

        private void ShowCompanyModal(Guid? guid = null)
        {
            if (guid.HasValue)
            {
                ModalService.Show<CompanyModal>(
                    ModalType.Center,
                    parameters: new Dictionary<string, object> { [nameof(CompanyModal.CompanyId)] = guid }
                );
            }
            else
            {
                ModalService.Show<CompanyModal>(ModalType.Center);
            }
        }

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
            _companies.Remove(company);
            --_totalCount;
            StateHasChanged();
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
