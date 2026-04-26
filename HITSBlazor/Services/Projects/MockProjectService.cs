using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Utils.Mocks.Projects;

namespace HITSBlazor.Services.Projects
{
    public class MockProjectService : IProjectService
    {
        public async Task<ListDataResponse<Project>> GetProjectsByQueryAsync(
            int page,
            string? searchText,
            ProjectStatus? selectedStatus
        ) => MockProjects.GetAllProjects(
            page, searchText: searchText, selectedStatus: selectedStatus
        );

        public async Task<List<Project>> GetAllActiveProjects(Guid userId)
            => MockProjects.GetActiveProjects(userId);
    }
}
