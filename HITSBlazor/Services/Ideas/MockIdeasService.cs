using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Utils;
using HITSBlazor.Utils.Mocks.Ideas;

namespace HITSBlazor.Services.Ideas
{
    public class MockIdeasService : IIdeasService
    {
        private List<Idea> _ideas = [];

        public async Task<ServiceResponse<List<Idea>>> GetAllIdeasAsync()
        {
            _ideas = MockIdeas.GetAllIdeas();

            return ServiceResponse<List<Idea>>.Success(_ideas);
        }

        public async Task<List<Idea>> GetIdeasByStatusAsync(params IdeaStatusType[] statusTypes)
            => [.. _ideas.Where(i => statusTypes.Contains(i.Status))];
    }
}
