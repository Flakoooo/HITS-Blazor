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

        private HashSet<User> _companyUsers = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (CompanyId.HasValue)
            {
                var company = await CompanyService.GetCompanyByIdAsync(CompanyId.Value);
                CompanyName = company?.Name ?? string.Empty;
                SelectedOwner = company?.Owner;
                _companyUsers = company?.Members.ToHashSet() ?? [];
            }

            _isLoading = false;
        }

        private void SelectUser(User user) => _companyUsers.Add(user);

        private void UnSelectUser(User user) => _companyUsers.Remove(user);

        private bool CheckValidValues()
        {
            if (string.IsNullOrWhiteSpace(CompanyName)) return false;
            if (SelectedOwner is null) return false;
            if (_companyUsers.Count == 0) return false;

            return true;
        }

        private async Task SendCompany()
        {
            _submitting = true;

            if (!CheckValidValues())
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
