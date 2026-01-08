using HITSBlazor.Models.Projects.Entities;

namespace HITSBlazor.Services.Projects
{
    public interface IProjectService
    {
        Task<List<Project>> GetAllActiveProjects(Guid userId);
    }
}
