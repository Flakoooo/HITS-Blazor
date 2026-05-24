using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Models.Users.Requests;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Users;
using HITSBlazor.Utils.Validation;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.CenterModals.UpdateUserModal
{
    public partial class UpdateUserModal
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private IUserService UserService { get; set; } = null!;

        [Parameter]
        public User UserForUpdate { get; set; } = new();

        private UpdateUserRequest UpdateUserRequest { get; set; } = new();

        private bool _submitting = false;
        private bool _showRoles = false;

        private readonly Dictionary<string, string> _errors = [];

        protected override async Task OnInitializedAsync()
        {
            UpdateUserRequest.Id = UserForUpdate.Id;
            UpdateUserRequest.Email = UserForUpdate.Email;
            UpdateUserRequest.FirstName = UserForUpdate.FirstName;
            UpdateUserRequest.LastName = UserForUpdate.LastName;
            UpdateUserRequest.Telephone = UserForUpdate.Telephone;
            UpdateUserRequest.StudyGroup = UserForUpdate.StudyGroup;
            UpdateUserRequest.Roles = [.. UserForUpdate.Roles];
        }

        private async Task OnRoleTypeChanged(RoleType role)
        {
            if (!UserForUpdate.Roles.Remove(role))
                UserForUpdate.Roles.Add(role);
        }

        private async Task UpdateUser()
        {
            _submitting = true;
            _errors.Clear();

            var emailResult = UserValidation.EmailValidation(UpdateUserRequest.Email);
            if (!emailResult.IsValid)
                _errors.Add("email", emailResult.Message);

            var firstNameResult = UserValidation.FirstNameValidation(UpdateUserRequest.FirstName);
            if (!firstNameResult.IsValid)
                _errors.Add("firstName", firstNameResult.Message);

            var lastNameResult = UserValidation.LastNameValidation(UpdateUserRequest.LastName);
            if (!firstNameResult.IsValid)
                _errors.Add("lastName", lastNameResult.Message);

            var telephoneResult = UserValidation.TelephoneValidation(UpdateUserRequest.Telephone);
            if (!telephoneResult.IsValid)
                _errors.Add("telephone", telephoneResult.Message);

            var studyGroupResult = UserValidation.StudyGroupValidation(UpdateUserRequest.StudyGroup);
            if (!telephoneResult.IsValid)
                _errors.Add("studyGroup", studyGroupResult.Message);

            if (_errors.Count > 0) return;

            if (await UserService.UpdateUser(UpdateUserRequest))
            {
                await ModalService.Close(ModalType.Center);
                return;
            }

            _submitting = false;
        }
    }
}
