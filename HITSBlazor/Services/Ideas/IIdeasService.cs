using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;
using HITSBlazor.Utils;

namespace HITSBlazor.Services.Ideas
{
    public interface IIdeasService
    {
        Task<ServiceResponse<List<Idea>>> GetAllIdeasAsync();
        Task<List<Idea>> GetIdeasByStatusAsync(params IdeaStatusType[] statusTypes);
        Task<ServiceResponse<bool>> CreateNewIdea(IdeasCreateModel ideasCreateModel);
    }
}
