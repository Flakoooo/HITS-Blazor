using HITSBlazor.Components.Modals.RightSideModals.ProfileModal;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils.Mocks.Users;
using Microsoft.AspNetCore.Components.Forms;

namespace HITSBlazor.Services.Profiles
{
    public class MockProfileService(
        IAuthService authService,
        GlobalNotificationService globalNotificationService
    ) : IProfileService
    {
        private readonly IAuthService _authService = authService;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<string?>? OnUserAvatarHasChanged;

        private string? _userAvatar = null;
        private Guid? _verificationGuid;
        private string? _newEmail = string.Empty;

        public async Task<Profile?> GetUserProifleAsync(Guid userId)
            => MockProfiles.GetUserProfileByUserId(userId);

        public async Task<string?> GetUserProifleAvatarAsync(Guid userId, bool refresh = false)
        {
            if (string.IsNullOrWhiteSpace(_userAvatar))
            {
                _globalNotificationService.ShowError("Mock данные не позволяют получать аватары пользователей. Выберите (обновите) изображение");
            }
            else
            {
                OnUserAvatarHasChanged?.Invoke(_userAvatar);
                return _userAvatar;
            }

            return string.Empty;
        }

        public async Task<bool> UpdateProfileUserDataAsync(UserDataForm userDataForm) 
            => await _authService.UpdateCurrentUser(
                firstName: userDataForm.FirstName,
                lastName: userDataForm.LastName,
                studyGroup: userDataForm.StudyGroup,
                telephone: userDataForm.Telephone
            );

        public async Task<bool> UpdateProfileAvatarAsync(IBrowserFile avatar)
        {
            byte[] fileBytes = [];
            try
            {
                using (var stream = avatar.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024))
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }
            }
            catch (Exception)
            {
                _globalNotificationService.ShowError("Непредвиденная ошибка. Попробуйте другой файл или повторите попытку позже");
            }

            if (fileBytes.Length == 0) return false;

            var base64 = Convert.ToBase64String(fileBytes);
            string avatarDataUrl = $"data:{avatar.ContentType};base64,{base64}";
            _userAvatar = avatarDataUrl;

            return true;
        }

        public async Task<Guid> SendUpdateEmailRequestAsync(string newEmail)
        {
            var email = newEmail.Trim();
            var atIndex = email.IndexOf('@');

            if (atIndex <= 0 || atIndex == email.Length - 1 || email.Count(c => c == '@') != 1)
            {
                _globalNotificationService.ShowError("Неверный формат почты");
                return Guid.Empty;
            }

            _verificationGuid = Guid.NewGuid();
            _newEmail = email;
            _globalNotificationService.ShowSuccess("На указанную почту были отправлены инструкции");
            return _verificationGuid.Value;
        }

        public async Task<bool> UpdateEmailConfirmAsync(Guid updateVerificationCode, string code)
        {
            if (updateVerificationCode != _verificationGuid)
            {
                _globalNotificationService.ShowError("Заявка на смену почты недействительна");
                return false;
            }

            if (code.Length < 6)
            {
                _globalNotificationService.ShowError("Код должен состоять из 6 цифр");
                return false;
            }

            if (code != "123456")
            {
                _globalNotificationService.ShowError("Указан неверный код");
                return false;
            }

            await _authService.UpdateCurrentUser(email: _newEmail);
            _verificationGuid = null;
            _newEmail = null;
            _globalNotificationService.ShowSuccess("Успешное обновление почты");
            return true;
        }
    }
}
