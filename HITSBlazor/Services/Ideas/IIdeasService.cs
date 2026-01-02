using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Utils;

namespace HITSBlazor.Services.Ideas
{
    public interface IIdeasService
    {
        Task<ServiceResponse<List<Idea>>> GetAllIdeasAsync();
    }
}
