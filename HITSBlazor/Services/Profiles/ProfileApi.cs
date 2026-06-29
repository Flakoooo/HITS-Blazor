using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HITSBlazor.Services.Profiles
{
    public class ProfileApi(
        IHttpClientFactory httpClientFactory, 
        ILogger<ProfileApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _profilePath = "/api/profile";

        private const string GET_PROFILE_OPERATION = "GetProfile";
        private const string GET_PROFILE_AVATAR_OPERATION = "GetProfileAvatar";
        private const string UPDATE_PROFILE_OPERATION = "UpdateProfile";
        private const string UPDATE_PROFILE_AVATAR_OPERATION = "UpdateProfileAvatar";
        private const string REQUEST_TO_UPDATE_EMAIL_OPERATION = "RequestToUpdateEmail";
        private const string CONFIRM_UPDATE_EMAIL_OPERATION = "ConfirmUpdateEmail";

        public async Task<ServiceResponse<Profile?>> GetProfileAsync(Guid userId)
        {
            string path = $"{_profilePath}/{userId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var users = await response.Content.ReadFromJsonAsync<Profile>(Settings.BaseJsonOptions);
                    if (users is null)
                    {
                        LogFail(GET_PROFILE_OPERATION, response.StatusCode, "Error when parse profile");

                        return ServiceResponse<Profile?>.Failure("Не удалось получить профиль", response.StatusCode);
                    }

                    return ServiceResponse<Profile?>.Success(users);
                },
                operationName: GET_PROFILE_OPERATION
            );
        }

        public async Task<ServiceResponse<string?>> GetProfileAvatarAsync(Guid userId)
        {
            string path = $"/api/images/avatar/{userId}.webp";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    var base64 = Convert.ToBase64String(bytes);
                    string avatarDataUrl = $"data:image/webp;base64,{base64}";

                    return ServiceResponse<string?>.Success(avatarDataUrl);
                },
                operationName: GET_PROFILE_AVATAR_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> UpdateProfileAsync(string firstName, string lastName, string studyGroup, string telephone)
        {
            var content = SerializeData(new
            {
                FirstName = firstName,
                LastName = lastName,
                StudyGroup = studyGroup,
                Telephone = telephone
            });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(_profilePath, content),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(UPDATE_PROFILE_OPERATION, response.StatusCode, "Error when update profile");

                        return ServiceResponse<string>.Failure("Не удалось обновить профиль", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: UPDATE_PROFILE_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> UpdateProfileAvatarAsync(byte[] fileBytes, string fileName, string contentType)
        {
            string path = $"{_profilePath}/avatar";

            var content = new MultipartFormDataContent();
            var byteContent = new ByteArrayContent(fileBytes);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Add(byteContent, "avatar", fileName);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(path, content),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(UPDATE_PROFILE_AVATAR_OPERATION, response.StatusCode, "Error when update profile avatar");

                        return ServiceResponse<string>.Failure("Не удалось обновить аватар профиля", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: UPDATE_PROFILE_AVATAR_OPERATION
            );
        }

        public async Task<ServiceResponse<Guid?>> SendUpdateEmailRequestAsync(string newEmail)
        {
            string path = $"{_profilePath}/email/verification/{newEmail}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(path, null),
                successHandler: async response =>
                {
                    var id = await response.Content.ReadFromJsonAsync<IdResponse>(Settings.BaseJsonOptions);
                    if (id is null)
                    {
                        LogFail(REQUEST_TO_UPDATE_EMAIL_OPERATION, response.StatusCode, "Error when send change email request");

                        return ServiceResponse<Guid?>.Failure("Не удалось отправить запрос на смену почты", response.StatusCode);
                    }

                    return ServiceResponse<Guid?>.Success(id.Id);
                },
                operationName: REQUEST_TO_UPDATE_EMAIL_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> ConfirmUpdateEmailRequestAsync(Guid verifiactionId, string code)
        {
            string path = $"{_profilePath}/email";
            var content = SerializeData(new { Id = verifiactionId, Code = code });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, content),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(CONFIRM_UPDATE_EMAIL_OPERATION, response.StatusCode, "Error when change email");

                        return ServiceResponse<string>.Failure("Не удалось сменить почту", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: CONFIRM_UPDATE_EMAIL_OPERATION
            );
        }
    }
}
