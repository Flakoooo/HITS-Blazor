using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Utils.Mocks.Projects;

namespace HITSBlazor.Services.Projects
{
    public class MockProjectService : IProjectService
    {
        public async Task<List<Project>> GetAllActiveProjects(Guid userId)
            => MockProjects.GetActiveProjects(userId);
    }
}
