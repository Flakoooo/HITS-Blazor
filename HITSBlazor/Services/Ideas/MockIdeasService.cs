using HITSBlazor.Components.Modals.RightSideModals.IdeaModal;
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
using HITSBlazor.Utils.Models;

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
                return new ListDataResponse<Idea>();

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

        public async Task<bool> UpdateCheckedIdeaAsync(Guid ideaId)
        {
            if (!MockIdeas.CheckIdea(ideaId)) return false;

            OnIdeaHasOpened?.Invoke(ideaId, true);
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

        //Ratings
        public async Task<List<Rating>> GetIdeaRatingsAsync(Guid ideaId)
            => MockRatings.GetIdeaRatingById(ideaId);

        public async Task<bool> SendRatingAsync(RatingRequest request, bool isConfirmed, List<Rating>? ideasRatings)
        {
            if (isConfirmed && ideasRatings is null)
            {
                string errorText = "При подстверждении рейтинга необхдимо также указать значение \"ideasRatings\"";
                throw new ArgumentNullException(errorText);
            }


            if (isConfirmed)
            {
                if (!MockRatings.UpdateOrConfirmRating(request, isConfirmed))
                {
                    _globalNotificationService.ShowError("Не удалось подтвердить рейтинг");
                    return false;
                }

                var rating = ideasRatings!.FirstOrDefault(r => r.Id == request.Id);
                if (rating is not null)
                {
                    rating.MarketValue = request.MarketValue;
                    rating.Originality = request.Originality;
                    rating.TechnicalRealizability = request.TechnicalRealizability;
                    rating.Suitability = request.Suitability;
                    rating.Budget = request.Budget;
                    rating.IsConfirmed = true;

                    if (ideasRatings!.Count == ideasRatings!.Count(r => r.IsConfirmed))
                        OnIdeasStatusHasChanged?.Invoke(rating.IdeaId, IdeaStatusType.Confirmed);
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
