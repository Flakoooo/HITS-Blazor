using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Requests;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Utils;
using System.Net.Http.Json;

namespace HITSBlazor.Services.Tags
{
    public class TagApi(
        IHttpClientFactory httpClientFactory, 
        ILogger<TagApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _tagPath = "/api/tag";

        private const string GET_TAGS_OPERATION = "GetTags";
        private const string CREATE_TAG_OPERATION = "CreateTag";
        private const string UPDATE_TAG_OPERATION = "UpdateTag";
        private const string CONFIRM_TAG_OPERATION = "ConfirmTag";
        private const string DELETE_TAG_OPERATION = "DeleteTag";

        public async Task<ServiceResponse<ListDataResponse<Tag>>> GetTagsAsync(
            int page,
            int pageSize = 20,
            string? searchText = null,
            bool? confirmed = null
        )
        {
            string path = $"{_tagPath}/all{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            if (confirmed.HasValue)
                path += AddQuery("confirmed", confirmed.Value);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var tags = await response.Content.ReadFromJsonAsync<ListDataResponse<Tag>>(Settings.BaseJsonOptions);
                    if (tags is null)
                    {
                        LogFail(GET_TAGS_OPERATION, response.StatusCode, "Error when parse tag model");

                        return ServiceResponse<ListDataResponse<Tag>>.Failure("Не удалось получить теги", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<Tag>>.Success(tags);
                },
                operationName: GET_TAGS_OPERATION
            );
        }

        public async Task<ServiceResponse<Tag?>> CreateTagAsync(string name, string color, bool confirmed)
        {
            string path = $"{_tagPath}/add{(confirmed ? string.Empty : "/no-confirmed")}";
            var content = SerializeData(new { Name = name, Color = color });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(path, content),
                successHandler: async resposne =>
                {
                    var tag = await resposne.Content.ReadFromJsonAsync<Tag>(Settings.BaseJsonOptions);
                    if (tag is null)
                    {
                        LogFail(CREATE_TAG_OPERATION, resposne.StatusCode, "Error when create tag");

                        return ServiceResponse<Tag?>.Failure("не удалось создать тег", resposne.StatusCode);
                    }

                    return ServiceResponse<Tag?>.Success(tag);
                },
                operationName: CREATE_TAG_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> UpdateTagAsync(Guid tagId, UpdateTagRequest request)
        {
            var path = $"{_tagPath}/update/{tagId}";
            var content = SerializeData(request);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, content),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(UPDATE_TAG_OPERATION, response.StatusCode, "Error when update tag");

                        return ServiceResponse<string>.Failure("Не удалось обновить тег", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: UPDATE_TAG_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> ConfirmTagAsync(Guid tagId)
        {
            string path = $"{_tagPath}/confirm/{tagId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, null),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(CONFIRM_TAG_OPERATION, response.StatusCode, "Error when confirm tag");

                        return ServiceResponse<string>.Failure("Не удалось утвердить тег", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: CONFIRM_TAG_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> DeleteTagAsync(Guid tagId)
        {
            string path = $"{_tagPath}/delete/{tagId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.DeleteAsync(path),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(DELETE_TAG_OPERATION, response.StatusCode, "Error when вудуеу tag");

                        return ServiceResponse<string>.Failure("Не удалось удалить тег", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: DELETE_TAG_OPERATION
            );
        }
    }
}
