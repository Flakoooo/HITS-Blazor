using HITSBlazor.Components.Modals.RightSideModals.IdeaModal;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Ideas;
using HITSBlazor.Utils.Models;

namespace HITSBlazor.Services.Ideas
{
    //TODO: реализовать получение идей постранично, подгружая нужное количество
    public class MockIdeasService(IAuthService authService, GlobalNotificationService globalNotificationService) : IIdeasService
    {
        private readonly IAuthService _authService = authService;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action? OnIdeasStateChanged;

        private HashSet<RoleType> _acceptableRoles =
        [
            RoleType.Initiator,
            RoleType.Member,
            RoleType.ProjectOffice,
            RoleType.Expert,
            RoleType.Admin,
            RoleType.Teacher
        ];

        public async Task<List<Idea>> GetIdeasAsync(
            int page,
            string? searchText,
            HashSet<IdeaStatusType>? statusTypes
        )
        {
            var activeRole = _authService.CurrentUser?.Role;
            if (activeRole is null || !_acceptableRoles.Contains((RoleType)activeRole)) return [];

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

        public async Task<Idea?> GetIdeaByIdAsync(Guid id) => MockIdeas.GetIdeaById(id);
        public async Task<int> GetTotalIdeaCount(
            string? searchText, HashSet<IdeaStatusType>? statusTypes
        )
        {
            var activeRole = _authService.CurrentUser?.Role;
            if (activeRole is null || !_acceptableRoles.Contains((RoleType)activeRole)) return 0;

            if (activeRole is RoleType.Initiator && _authService.CurrentUser is not null)
                return MockIdeas.GetTotalInitiatorIdeasCount(
                    _authService.CurrentUser.Id,
                    searchText: searchText,
                    statusTypes: statusTypes
                );

            return MockIdeas.GetTotalIdeasCount(
                    searchText: searchText,
                    statusTypes: statusTypes
                );
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

        //TODO: как то в UI отслеживать изменение, походу передавать туда Id идеи и true
        public async Task<bool> UpdateCheckedIdeaAsync(Guid ideaId)
        {
            if (!MockIdeas.CheckIdea(ideaId)) return false;

            OnIdeasStateChanged?.Invoke();
            return true;
        }

        public async Task<bool> UpdateIdeaAsync(Guid ideaId, IdeasCreateModel ideasCreateModel)
        {
            var idea = MockIdeas.GetIdeaById(ideaId);
            if (idea is null)
            {
                _globalNotificationService.ShowError("Редактируемая идея не найдена");
                return false;
            }

            if (!MockIdeas.UpdateIdea(ideaId, ideasCreateModel))
            {
                _globalNotificationService.ShowError("Не удалось обновить идею");
                return false;
            }

            _globalNotificationService.ShowSuccess("Идея изменена");

            return true;
        }

        //TODO: как то обновлять в UI статус идеи
        public async Task<bool> UpdateIdeaStatusAsync(Guid ideaId, IdeaStatusType ideaStatus)
        {
            var updatedIdea = MockIdeas.UpdateIdeaStatus(ideaId, ideaStatus);
            if (updatedIdea is null)
            {
                _globalNotificationService.ShowError($"Не удалось сменить статус идеи на {EnumViewModel<IdeaStatusType>.GetTranslation(ideaStatus)}");
                return false;
            }

            OnIdeasStateChanged?.Invoke();
            return true;
        }

        //TODO: как то удалять из UI
        public async Task<bool> DeleteIdeaAsync(Idea idea)
        {
            if (!MockIdeas.DeleteIdea(idea))
            {
                _globalNotificationService.ShowError("Не удалось удалить идею");
                return false;
            }

            return true;
        }

        //Skills
        public async Task<List<Skill>> GetAllIdeaSkillsAsync(Guid ideaId)
            => MockIdeaSkills.GetIdeaSkillsByIdeaId(ideaId);

        public async Task CreateOrUpdateIdeasSkills(Guid ideaId, List<Skill> skills)
            => MockIdeaSkills.CreateOrUpdateIdeasSkills(ideaId, skills);

        //Ratings
        public async Task<List<Rating>> GetIdeaRatingsAsync(Guid ideaId)
            => MockRatings.GetIdeaRatingById(ideaId);

        public async Task<bool> SendRatingAsync(RatingRequest request, bool isConfirmed)
        {
            if (isConfirmed)
            {
                if (!MockRatings.UpdateOrConfirmRating(request, isConfirmed))
                {
                    _globalNotificationService.ShowError("Не удалось подтвердить рейтинг");
                    return false;
                }

                _globalNotificationService.ShowSuccess("Рейтинг успешно подтвержден");
            }
            else
            {
                if (!MockRatings.UpdateOrConfirmRating(request))
                {
                    _globalNotificationService.ShowError("Не удалось сохранить рейтинг");
                    return false;
                }

                _globalNotificationService.ShowSuccess("Рейтинг успешно сохранен");
            }

            return true;
        }

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
