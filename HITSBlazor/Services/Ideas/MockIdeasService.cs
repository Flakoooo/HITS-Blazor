using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Utils;
using HITSBlazor.Utils.Mocks.Ideas;

namespace HITSBlazor.Services.Ideas
{
    public class MockIdeasService : IIdeasService
    {
        private List<Idea> _ideas = [];

        public async Task<ServiceResponse<List<Idea>>> GetAllIdeasAsync()
        {
            if (_ideas.Count == 0)
                _ideas = MockIdeas.GetAllIdeas();

            return ServiceResponse<List<Idea>>.Success(_ideas);
        }
    }
}
