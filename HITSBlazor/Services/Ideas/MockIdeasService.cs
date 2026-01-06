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

        public async Task<ServiceResponse<List<Idea>>> GetAllIdeasAsync()
        {
            _ideas = MockIdeas.GetAllIdeas();

            return ServiceResponse<List<Idea>>.Success(_ideas);
        }

        public async Task<List<Idea>> GetIdeasByStatusAsync(params IdeaStatusType[] statusTypes)
            => [.. _ideas.Where(i => statusTypes.Contains(i.Status))];

        public async Task<ServiceResponse<bool>> CreateNewIdea(IdeasCreateModel ideasCreateModel)
        {
            if (_authService.CurrentUser is null)
            {
                _globalNotificationService.ShowError("Пользователь не найден");
                return ServiceResponse<bool>.Failure("Пользователь не найден");
            }

            ideasCreateModel.Initiator = _authService.CurrentUser;

            return ServiceResponse<bool>.Success(true);
        }
    }
}
