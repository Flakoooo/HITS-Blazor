using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;
using HITSBlazor.Utils;

namespace HITSBlazor.Services.Ideas
{
    public interface IIdeasService
    {
        Task<List<Idea>> GetAllIdeasAsync(bool isApiResponse = false, string? seacrhText = null);
        Task<List<Idea>> GetIdeasByStatusAsync(string? seacrhText = null, params IdeaStatusType[] statusTypes);
        Task<Idea?> GetIdeaByIdAsync(Guid id);
        Task<ServiceResponse<bool>> CreateNewIdea(IdeasCreateModel ideasCreateModel);
    }
}
