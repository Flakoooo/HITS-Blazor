using HITSBlazor.Components.Modals.RightSideModals.ProfileModal;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Users;
using HITSBlazor.Utils.Mocks.Users;
using HITSBlazor.Utils.Validation;

namespace HITSBlazor.Services.Profiles
{
    public class ProfileService(
        IAuthService authService,
        ProfileApi profileApi,
        ILogger<ProfileService> logger,
        GlobalNotificationService globalNotificationService
    ) : IProfileService
    {
        private readonly IAuthService _authService = authService;
        private readonly ProfileApi _profileApi = profileApi;
        private readonly ILogger<ProfileService> _logger = logger;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        private Guid? _verificationGuid;
        private string? _newEmail = string.Empty;

        public async Task<Profile?> GetUserProifleAsync(Guid userId)
        {
            var result = await _profileApi.GetProfileAsync(userId);

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get profile failed: {Error}", result.Message);
            }

            return result.Response;
        }

        public async Task<bool> UpdateProfileUserDataAsync(UserDataForm userDataForm)
        {
            var result = await _profileApi.UpdateProfileAsync(
                userDataForm.FirstName,
                userDataForm.LastName,
                userDataForm.StudyGroup,
                userDataForm.Telephone
            );

            if (result.IsSuccess && result.Response is not null)
            {
                _globalNotificationService.ShowSuccess(result.Response);
                return true;
            }
            
            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update profile failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task<Guid> SendUpdateEmailRequestAsync(string newEmail)
        {
            var email = newEmail.Trim();

            var emailValidResult = UserValidation.EmailValidation(email);
            if (!emailValidResult.IsValid)
            {
                _globalNotificationService.ShowError(emailValidResult.Message);
                return Guid.Empty;
            }

            var result = await _profileApi.SendUpdateEmailRequestAsync(email);
            if (result.IsSuccess && result.Response.HasValue)
            {
                _verificationGuid = result.Response.Value;
                _newEmail = email;
                _globalNotificationService.ShowSuccess("На указанную почту были отправлены инструкции");
                return _verificationGuid.Value;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Send change email request failed: {Error}", result.Message);
            }

            return Guid.Empty;
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

            var result = await _profileApi.ConfirmUpdateEmailRequestAsync(updateVerificationCode, code);
            if (result.IsSuccess && result.Response is not null)
            {
                _verificationGuid = null;
                _newEmail = null;
                _globalNotificationService.ShowSuccess(result.Response);
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Confirm email request failed: {Error}", result.Message);
            }

            return false;
        }
    }
}
