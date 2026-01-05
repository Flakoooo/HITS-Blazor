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

        [Parameter] public Guid CompanyId { get; set; }
        [Parameter] public List<User> AllContactPersons { get; set; } = [];
        [Parameter] public User? SelectedContactPerson { get; set; }
        [Parameter] public EventCallback<User?> SelectedContactPersonChanged { get; set; }

        [Parameter] public Func<string, Task<List<User>>>? SearchFunction { get; set; }

        private ElementReference inputRef;
        private bool IsOpen { get; set; }
        private string searchText = "";
        private List<User> FilteredContactPersons { get; set; } = [];
        private DotNetObjectReference<ContactPersonDropdown>? dotNetHelper;

        private string SelectedContactPersonName =>
            SelectedContactPerson != null ?
            $"{SelectedContactPerson.FirstName} {SelectedContactPerson.LastName}" :
            string.Empty;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetHelper = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("dropdownManager.registerClickOutside", inputRef, dotNetHelper);
                FilteredContactPersons = [.. AllContactPersons];
            }
        }

        private async Task OpenDropdown()
        {
            if (!IsOpen)
            {
                IsOpen = true;
                if (string.IsNullOrWhiteSpace(searchText))
                    FilteredContactPersons = [.. AllContactPersons];
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
                FilteredContactPersons = await SearchFunction(searchText);
        }

        private void OnInputKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
                IsOpen = false;
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
                await JSRuntime.InvokeVoidAsync("dropdownManager.unregisterClickOutside", inputRef);
                dotNetHelper.Dispose();
            }
        }
    }
}
