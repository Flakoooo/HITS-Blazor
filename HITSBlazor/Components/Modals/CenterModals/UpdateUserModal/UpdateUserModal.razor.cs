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

        private bool _submitted = false;
        private bool _submitting = false;
        private bool _showRoles = false;

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

        //TODO: ПОДУМАТЬ НАД ВАЛИДАЦИЕЙ!!!!
        private async Task UpdateUser()
        {
            _submitting = true;
            _submitted = false;

            bool isValid = true;

            if (!UserValidation.EmailValidation(UpdateUserRequest.Email).IsValid) isValid = false;
            if (!UserValidation.FirstNameValidation(UpdateUserRequest.FirstName).IsValid) isValid = false;
            if (!UserValidation.LastNameValidation(UpdateUserRequest.LastName).IsValid) isValid = false;
            if (!UserValidation.TelephoneValidation(UpdateUserRequest.Telephone).IsValid) isValid = false;
            if (!UserValidation.StudyGroupValidation(UpdateUserRequest.StudyGroup).IsValid) isValid = false;

            if (isValid && await UserService.UpdateUser(UpdateUserRequest))
            {
                await ModalService.Close(ModalType.Center);
                return;
            }

            _submitted = true;
            _submitting = false;
        }
    }
}
