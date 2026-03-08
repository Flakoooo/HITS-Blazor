using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Services.Companies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace HITSBlazor.Components.InputDropdowns.CompanyDropdown
{
    public partial class CompanyDropdown : IAsyncDisposable
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        [Inject]
        private ICompanyService CompanyService { get; set; } = null!;

        [Parameter] public string Placeholder { get; set; } = "Выберите заказчика";
        [Parameter] public string RadioGroupName { get; set; } = "company";

        [Parameter] public Company? SelectedCompany { get; set; }
        [Parameter] public EventCallback<Company?> SelectedCompanyChanged { get; set; }

        [Parameter] public Func<string, Task<List<Company>>>? SearchFunction { get; set; }

        [Parameter]
        public bool NeedValidation { get; set; } = false;

        [Parameter]
        public string? ErrorMessage { get; set; } = "Поле не заполнено";

        private ElementReference inputRef;
        private DotNetObjectReference<CompanyDropdown>? dotNetHelper;

        private bool _isOpen = false;
        private bool _showError = false;

        private List<Company> _companies = [];
        private string _searchText = string.Empty;

        private string SelectedCompanyName => SelectedCompany?.Name ?? string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadCompaniesAsync();
        }

        protected override void OnParametersSet()
        {
            _showError = NeedValidation && SelectedCompany is null;
            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetHelper = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("listDropdown.registerClickOutside", inputRef, dotNetHelper);
            }
        }

        private async Task LoadCompaniesAsync()
        {
            _companies = await CompanyService.GetCompaniesAsync(
                searchText: _searchText
            );
            StateHasChanged();
        }

        private async Task OpenDropdown()
        {
            if (!_isOpen) _isOpen = true;
        }

        private void ToggleDropdown() => _isOpen = !_isOpen;

        [JSInvokable]
        public void CloseDropdown()
        {
            _isOpen = false;
            InvokeAsync(StateHasChanged);
        }

        private async Task OnSearch(ChangeEventArgs e)
        {
            _searchText = (e.Value?.ToString() ?? "").Trim();
            await LoadCompaniesAsync();
        }

        private void OnInputKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
                _isOpen = false;
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
                await JSRuntime.InvokeVoidAsync("listDropdown.unregisterClickOutside", inputRef);
                dotNetHelper.Dispose();
            }
        }
    }
}
