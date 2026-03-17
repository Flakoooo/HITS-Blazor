using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Modals.CenterModals.CompanyModal;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Services.Companies;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;

namespace HITSBlazor.Pages.Admin.AllCompanies
{
    [Authorize]
    [Route("/admin/companies")]
    public partial class AllCompanies : IDisposable
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

            CompanyService.OnCompaniesStateChanged += LoadCompaniesAsync;
            CompanyService.OnCompaniesStateUpdated += StateHasChanged;
            await LoadCompaniesAsync();

            _isLoading = false;
        }

        private async Task LoadCompaniesAsync()
        {
            _companies = await CompanyService.GetCompaniesAsync(
                _searchText
            );
            StateHasChanged();
        }

        private async Task SeacrhCompany(string value)
        {
            _searchText = value;
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
            if (context.Action == MenuAction.Edit)
            {
                if (context.Item is Guid guid)
                    ShowCompanyModal(guid);
            }
            else if (context.Action == MenuAction.Delete)
            {
                if (context.Item is not Company company || !await CompanyService.DeleteCompanyAsync(company))
                    return;

                _companies.Remove(company);
            }
        }

        public void Dispose()
        {
            CompanyService.OnCompaniesStateChanged -= LoadCompaniesAsync;
            CompanyService.OnCompaniesStateUpdated -= StateHasChanged;
        }
    }
}
