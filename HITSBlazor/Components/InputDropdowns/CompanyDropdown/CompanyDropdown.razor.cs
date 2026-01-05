using HITSBlazor.Models.Common.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace HITSBlazor.Components.InputDropdowns.CompanyDropdown
{
    public partial class CompanyDropdown : IAsyncDisposable
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        [Parameter] public string Placeholder { get; set; } = "Выберите заказчика";
        [Parameter] public string RadioGroupName { get; set; } = "company";

        [Parameter] public List<Company> AllCompanies { get; set; } = [];
        [Parameter] public Company? SelectedCompany { get; set; }
        [Parameter] public EventCallback<Company?> SelectedCompanyChanged { get; set; }

        [Parameter] public Func<string, Task<List<Company>>>? SearchFunction { get; set; }

        private ElementReference inputRef;
        private bool IsOpen { get; set; }
        private string searchText = "";
        private List<Company> FilteredCompanies { get; set; } = [];
        private DotNetObjectReference<CompanyDropdown>? dotNetHelper;

        private string SelectedCompanyName => SelectedCompany?.Name ?? string.Empty;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetHelper = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("dropdownManager.registerClickOutside", inputRef, dotNetHelper);
                FilteredCompanies = [.. AllCompanies];
            }
        }

        private async Task OpenDropdown()
        {
            if (!IsOpen)
            {
                IsOpen = true;
                if (string.IsNullOrWhiteSpace(searchText))
                    FilteredCompanies = [.. AllCompanies];
            }
        }

        private void ToggleDropdown() => IsOpen = !IsOpen;

        [JSInvokable]
        public void CloseDropdown()
        {
            IsOpen = false;
            InvokeAsync(StateHasChanged);
        }

        private async Task OnSearch(ChangeEventArgs e)
        {
            searchText = (e.Value?.ToString() ?? "").Trim();

            if (SearchFunction != null)
                FilteredCompanies = await SearchFunction(searchText);
        }

        private void OnInputKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
                IsOpen = false;
        }

        private async Task OnElementChange(Company company)
        {
            SelectedCompany = company;
            await SelectedCompanyChanged.InvokeAsync(SelectedCompany);
        }

        private bool IsCompanySelected(Company company) => SelectedCompany?.Id == company.Id;

        public async ValueTask DisposeAsync()
        {
            if (dotNetHelper != null)
            {
                await JSRuntime.InvokeVoidAsync("dropdownManager.unregisterClickOutside", inputRef);
                dotNetHelper.Dispose();
            }
        }
    }
}
