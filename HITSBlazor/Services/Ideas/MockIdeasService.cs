using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils;
using HITSBlazor.Utils.Mocks.Ideas;

namespace HITSBlazor.Services.Ideas
{
    public class MockIdeasService(IAuthService authService, GlobalNotificationService globalNotificationService) : IIdeasService
    {
        private readonly IAuthService _authService = authService;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        private List<Idea> _ideas = [];

        public async Task<List<Idea>> GetAllIdeasAsync(bool isApiResponse, string? seacrhText)
        {
            if (isApiResponse)
                _ideas = MockIdeas.GetAllIdeas();

            return string.IsNullOrWhiteSpace(seacrhText) 
                ? _ideas 
                : [.. _ideas.Where(i => i.Name.Contains(seacrhText, StringComparison.CurrentCultureIgnoreCase))];
        }

        public async Task<List<Idea>> GetIdeasByStatusAsync(
            string? seacrhText, params IdeaStatusType[] statusTypes
        ) => string.IsNullOrWhiteSpace(seacrhText)
            ? [.. _ideas.Where(i => statusTypes.Contains(i.Status))]
            : [.. _ideas.Where(i => statusTypes.Contains(i.Status) && i.Name.Contains(seacrhText, StringComparison.CurrentCultureIgnoreCase))];

        public async Task<Idea?> GetIdeaByIdAsync(Guid id) => MockIdeas.GetIdeaById(id);

        public async Task<ServiceResponse<bool>> CreateNewIdea(IdeasCreateModel ideasCreateModel)
        {
            if (_authService.CurrentUser is null)
            {
                _globalNotificationService.ShowError("Пользователь не найден");
                return ServiceResponse<bool>.Failure("Пользователь не найден");
            }

            return ServiceResponse<bool>.Success(true);
        }
    }
}
