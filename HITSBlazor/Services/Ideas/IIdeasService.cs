using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;
using HITSBlazor.Utils;

namespace HITSBlazor.Services.Ideas
{
    public interface IIdeasService
    {
        Task<List<Idea>> GetIdeasAsync(string? searchText = null, IdeaStatusType[]? statusTypes = null);
        Task<Idea?> GetIdeaByIdAsync(Guid id);
        Task<ServiceResponse<bool>> CreateNewIdea(IdeasCreateModel ideasCreateModel);
    }
}
