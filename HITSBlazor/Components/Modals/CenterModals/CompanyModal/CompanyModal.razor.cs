using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Companies;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Users;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.CenterModals.CompanyModal
{
    public partial class CompanyModal
    {
        //TODO: сделать другую логику выбора учатсников
        [Inject]
        private IUserService UserService { get; set; } = null!;

        [Inject]
        private ICompanyService CompanyService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private GlobalNotificationService NotificationService { get; set; } = null!;

        [Parameter]
        public Guid? CompanyId { get; set; } = null!;

        private bool _isLoading = true;
        private bool _submitting = false;

        private string CompanyName { get; set; } = string.Empty;
        private User? SelectedOwner { get; set; }

        private List<User> _users = [];
        private List<User> _companyUsers = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (CompanyId.HasValue)
            {
                var company = await CompanyService.GetCompanyByIdAsync(CompanyId.Value);
                CompanyName = company?.Name ?? string.Empty;
                SelectedOwner = company?.Owner;
                _companyUsers = company?.Members ?? [];
                _users = [.. (await UserService.GetUsersAsync()).Where(u => !_companyUsers.Contains(u))];
            }
            else
            {
                _users = await UserService.GetUsersAsync();
            }

            _isLoading = false;
        }

        private void SelectUser(User user)
        {
            _companyUsers.Add(user);
            _users.Remove(user);
        }

        private void UnSelectUser(User user)
        {
            _companyUsers.Remove(user);
            _users.Add(user);
        }

        private async Task SendCompany()
        {
            _submitting = true;

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(CompanyName)) isValid = false;
            if (SelectedOwner is null) isValid = false;
            if (_companyUsers.Count == 0) isValid = false;

            if (!isValid)
            {
                NotificationService.ShowError("Заполнены не все поля");
                _submitting = false;
                return;
            }

            bool result;
#pragma warning disable CS8604 // Possible null reference argument.
            if (CompanyId.HasValue)
            {
                result = await CompanyService.UpdateCompanyAsync(
                    CompanyId.Value,
                    CompanyName,
                    SelectedOwner,
                    _companyUsers
                );
            }
            else
            {
                result = await CompanyService.CreateCompanyAsync(
                    CompanyName,
                    SelectedOwner,
                    _companyUsers
                );
            }
#pragma warning restore CS8604 // Possible null reference argument.

            if (result)
                await ModalService.Close(ModalType.Center);

            _submitting = false;
        }
    }
}
