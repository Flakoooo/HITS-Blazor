using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Models.Users.Requests;
using HITSBlazor.Services;
using HITSBlazor.Services.Invitation;
using HITSBlazor.Utils.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace HITSBlazor.Pages.Admin.InviteUsers
{
    [Authorize]
    [Route("admin/add-users")]
    public partial class InviteUsers
    {
        [Inject]
        private IInvitationService InvitationService { get; set; } = null!;

        [Inject]
        private GlobalNotificationService NotificationService { get; set; } = null!;

        private bool _showRoles = false;

        private bool _fileLoading = false;
        private bool _fileLoaded = false;
        private bool _submitted = false;
        private bool _submitting = false;

        private InviteUsersRequest InviteUsersRequest { get; set; } = new();

        private HashSet<RoleType> SelectedRoles { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            if (InviteUsersRequest.Emails.Count == 0) InviteUsersRequest.Emails.Add(string.Empty);
        }

        private void AddEmail() => InviteUsersRequest.Emails.Add(string.Empty);

        private void RemoveEmail(int index) => InviteUsersRequest.Emails.RemoveAt(index);

        private async Task HandleFileLoad(InputFileChangeEventArgs e)
        {
            try
            {
                _fileLoading = true;
                _fileLoaded = false;

                var file = e.File;
                if (file is null)
                {
                    _fileLoaded = false;
                    return;
                }

                using var reader = new StreamReader(file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024));
                var content = await reader.ReadToEndAsync();

                var rawEmails = content
                    .Replace("\r\n", "\n")
                    .Replace("\r", "\n")
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .SelectMany(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    .Where(email => !string.IsNullOrWhiteSpace(email))
                    .Select(email => email.Trim())
                    .ToList();

                if (rawEmails.Count == 0)
                {
                    NotificationService.ShowError("Файл не содержит email'ов");
                    _fileLoaded = false;
                    return;
                }

                var validEmails = new List<string>();
                var invalidEmails = new List<string>();

                foreach (var email in rawEmails)
                {
                    var validation = UserValidation.EmailValidation(email);
                    if (validation.IsValid)
                        validEmails.Add(email);
                    else
                        invalidEmails.Add(email);
                }

                if (InviteUsersRequest.Emails.Count == 1 && string.IsNullOrWhiteSpace(InviteUsersRequest.Emails[0]))
                    InviteUsersRequest.Emails.Clear();

                InviteUsersRequest.Emails.AddRange(invalidEmails);
                InviteUsersRequest.Emails.AddRange(validEmails);

                var message = $"Валидацию прошло {validEmails.Count} из {rawEmails.Count} email'ов.";

                if (invalidEmails.Count != 0)
                {
                    message += " Некорректные email'ы добавлены в начало списка.";
                    NotificationService.ShowError(message);
                }
                else
                {
                    NotificationService.ShowSuccess(message);
                }

                _fileLoaded = true;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                NotificationService.ShowError($"Ошибка при загрузке файла: {ex.Message}");
            }
            finally
            {
                _fileLoading = false;
            }
        }

        private async Task OnRoleTypeChanged(RoleType role)
        {
            if (!SelectedRoles.Remove(role))
                SelectedRoles.Add(role);
        }

        private async Task SendInvitations()
        {
            _submitting = true;
            _submitted = false;

            if (SelectedRoles.Count == 0)
            {
                NotificationService.ShowError("Выберите хотя бы одну роль");
                _submitted = true;
                _submitting = false;
                return;
            }

            bool isValid = true;
            foreach (var email in InviteUsersRequest.Emails)
            {
                if (!UserValidation.EmailValidation(email).IsValid)
                {
                    isValid = false;
                    break;
                }
            }

            if (!isValid)
            {
                NotificationService.ShowError("Имеются некорректные email");
                _submitted = true;
                _submitting = false;
                return;
            }

            InviteUsersRequest.Roles = [.. SelectedRoles];
            await InvitationService.SendInvitations(InviteUsersRequest);

            _submitted = true;
            _submitting = false;
        }
    }
}
