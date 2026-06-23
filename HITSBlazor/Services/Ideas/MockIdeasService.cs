using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils.EnumUIConverters;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Ideas;

namespace HITSBlazor.Services.Ideas
{
    public class MockIdeasService(IAuthService authService, GlobalNotificationService globalNotificationService) : IIdeasService
    {
        private readonly IAuthService _authService = authService;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<Idea>? OnIdeaHasDeleted;
        public event Action<Guid, bool>? OnIdeaHasOpened;
        public event Action<Guid, IdeaStatusType>? OnIdeasStatusHasChanged;

        private readonly HashSet<RoleType> _acceptableRoles =
        [
            RoleType.Initiator,
            RoleType.Member,
            RoleType.ProjectOffice,
            RoleType.Expert,
            RoleType.Admin,
            RoleType.Teacher
        ];

        public async Task<ListDataResponse<Idea>> GetIdeasAsync(
            int page,
            string? searchText,
            HashSet<IdeaStatusType>? statusTypes
        )
        {
            var activeRole = _authService.CurrentUser?.Role;
            if (activeRole is null || !_acceptableRoles.Contains((RoleType)activeRole)) 
                return new ListDataResponse<Idea>(0, []);

            if (activeRole is RoleType.Initiator && _authService.CurrentUser is not null)
                return MockIdeas.GetInitiatorIdeasByQueryParams(
                    _authService.CurrentUser.Id,
                    page,
                    searchText: searchText,
                    statusTypes: statusTypes
                );

            return MockIdeas.GetAllIdeasByQueryParams(
                    page,
                    searchText: searchText,
                    statusTypes: statusTypes
                );
        }

        public async Task<Idea?> GetIdeaByIdAsync(Guid id)
        {
            var idea = MockIdeas.GetIdeaById(id);
            OnIdeaHasOpened?.Invoke(id, true);
            return idea;
        }

        public async Task<Idea?> CreateNewIdeaAsync(IdeasCreateModel ideasCreateModel)
        {
            if (_authService.CurrentUser is null)
            {
                _globalNotificationService.ShowError("Пользователь не найден");
                return null;
            }

            var newIdea = MockIdeas.CreateNewIdea(ideasCreateModel, _authService.CurrentUser);
            if (newIdea is null)
            {
                _globalNotificationService.ShowError("Не удалось создать идею");
                return null;
            }

            if (ideasCreateModel.Status == IdeaStatusType.New)
                _globalNotificationService.ShowSuccess("Черновик идеи сохранен");
            else if (ideasCreateModel.Status == IdeaStatusType.OnApproval)
                _globalNotificationService.ShowSuccess("Идея отправлена на согласование");

            return newIdea;
        }

        public async Task<bool> UpdateIdeaAsync(IdeasCreateModel ideasCreateModel)
        {
            if (!ideasCreateModel.Id.HasValue)
            {
                _globalNotificationService.ShowError("Не удалось обновить идею");
                return false;
            }

            var idea = MockIdeas.GetIdeaById(ideasCreateModel.Id.Value);
            if (idea is null)
            {
                _globalNotificationService.ShowError("Редактируемая идея не найдена");
                return false;
            }

            if (!MockIdeas.UpdateIdea(ideasCreateModel))
            {
                _globalNotificationService.ShowError("Не удалось обновить идею");
                return false;
            }

            _globalNotificationService.ShowSuccess("Идея изменена");

            return true;
        }

        public void IdeasStatusHasUpdatedEvent(Guid ideaId, IdeaStatusType ideaStatus)
            => OnIdeasStatusHasChanged?.Invoke(ideaId, ideaStatus);

        public async Task<bool> UpdateIdeaStatusAsync(Guid ideaId, IdeaStatusType ideaStatus)
        {
            var updatedIdea = MockIdeas.UpdateIdeaStatus(ideaId, ideaStatus);
            if (updatedIdea is null)
            {
                _globalNotificationService.ShowError($"Не удалось сменить статус идеи на {EnumUIConverter.GetInfo(ideaStatus)}");
                return false;
            }

            OnIdeasStatusHasChanged?.Invoke(ideaId, ideaStatus);
            return true;
        }

        public async Task<bool> DeleteIdeaAsync(Idea idea)
        {
            if (!MockIdeas.DeleteIdea(idea))
            {
                _globalNotificationService.ShowError("Не удалось удалить идею");
                return false;
            }

            OnIdeaHasDeleted?.Invoke(idea);

            return true;
        }

        //Skills
        public async Task<List<Skill>> GetAllIdeaSkillsAsync(Guid ideaId)
            => MockIdeaSkills.GetIdeaSkillsByIdeaId(ideaId);

        public async Task CreateOrUpdateIdeasSkills(Guid ideaId, List<Skill> skills)
            => MockIdeaSkills.CreateOrUpdateIdeasSkills(ideaId, skills);

        //Comments
        public async Task<List<Comment>> GetIdeasCommentsAsync(Guid ideaId)
            => MockComments.GetIdeasCommentsByIdeaId(ideaId);

        public async Task<bool> DeleteCommentInIdeaAsync(Comment comment)
        {
            if (!MockComments.DeleteIdeasComment(comment))
            {
                _globalNotificationService.ShowError("Не удалось удалить комментарий");
                return false;
            }

            return true;
        }
    }
}
