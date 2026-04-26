using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;

namespace HITSBlazor.Services.Projects
{
    public interface IProjectService
    {
        Task<ListDataResponse<Project>> GetProjectsByQueryAsync(
            int page,
            string? searchText = null,
            ProjectStatus? selectedStatus = null
        );

        Task<List<Project>> GetAllActiveProjects(Guid userId);
    }
}
