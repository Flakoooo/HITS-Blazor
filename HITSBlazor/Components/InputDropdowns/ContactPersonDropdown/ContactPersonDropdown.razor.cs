using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Users.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace HITSBlazor.Components.InputDropdowns.ContactPersonDropdown
{
    public partial class ContactPersonDropdown : IAsyncDisposable
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        [Parameter] public Company SelectedCompany { get; set; } = new();
        [Parameter] public User? SelectedContactPerson { get; set; }
        [Parameter] public EventCallback<User?> SelectedContactPersonChanged { get; set; }

        private ElementReference inputRef;
        private DotNetObjectReference<ContactPersonDropdown>? dotNetHelper;

        private bool _isOpen = false;

        private string _searchText = string.Empty;

        private List<User> _contactPersons = [];

        private string SelectedContactPersonName =>
            SelectedContactPerson != null ? SelectedContactPerson.FullName : string.Empty;

        protected override void OnParametersSet()
        {
            _contactPersons = SelectedCompany.Users;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetHelper = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("listDropdown.registerClickOutside", inputRef, dotNetHelper);
            }
        }

        private void SearchContactPersons()
        {
            if (string.IsNullOrWhiteSpace(_searchText))
                _contactPersons = SelectedCompany.Users;
            else
                _contactPersons = [.. SelectedCompany.Users.Where(u => u.FullName.Contains(_searchText))];
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

            SearchContactPersons();
        }

        private void OnInputKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Escape") _isOpen = false;
        }

        private async Task OnElementChange(User contactPerson)
        {
            SelectedContactPerson = contactPerson;
            await SelectedContactPersonChanged.InvokeAsync(SelectedContactPerson);
        }

        private bool IsContactPersonSelected(User contactPerson) => SelectedContactPerson?.Id == contactPerson.Id;

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
