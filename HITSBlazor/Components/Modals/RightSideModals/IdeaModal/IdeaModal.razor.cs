using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Services.Modal;
using HITSBlazor.Utils;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.RightSideModals.IdeaModal
{
    public partial class IdeaModal
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Inject]
        private IIdeasService IdeasService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public Guid IdeaId { get; set; }

        private bool isLoading = true;
        private bool isRatingSaving = false;
        private bool isRatingSaved = false;
        private bool isRatingConfirming = false;

        private User? CurrentUser { get; set; } = null;
        private Idea? CurrentIdea { get; set; } = null;
        private List<Skill> IdeaSkills { get; set; } = [];
        private List<Rating> IdeaRatings { get; set; } = [];
        private List<Comment> IdeaComments { get; set; } = [];

        private List<IdeaModalItem> ideaData = [];

        private RatingRequest? _expertRating = null;


        private string MarketValue
        {
            get => _expertRating?.MarketValue.ToString() ?? string.Empty;
            set 
            {
                if (int.TryParse(value, out int intValue) && _expertRating?.MarketValue != intValue)
                {
                    _expertRating?.MarketValue = intValue;
                    UpdateRatingScore();
                }
            }
        }

        private string Originality
        {
            get => _expertRating?.Originality.ToString() ?? string.Empty;
            set
            {
                if (int.TryParse(value, out int intValue) && _expertRating?.Originality != intValue)
                {
                    _expertRating?.Originality = intValue;
                    UpdateRatingScore();
                }
            }
        }

        private string TechnicalRealizability
        {
            get => _expertRating?.TechnicalRealizability.ToString() ?? string.Empty;
            set
            {
                if (int.TryParse(value, out int intValue) && _expertRating?.TechnicalRealizability != intValue)
                {
                    _expertRating?.TechnicalRealizability = intValue;
                    UpdateRatingScore();
                }
            }
        }

        private string Suitability
        {
            get => _expertRating?.Suitability.ToString() ?? string.Empty;
            set
            {
                if (int.TryParse(value, out int intValue) && _expertRating?.Suitability != intValue)
                {
                    _expertRating?.Suitability = intValue;
                    UpdateRatingScore();
                }
            }
        }

        private string Budget
        {
            get => _expertRating?.Budget.ToString() ?? string.Empty;
            set
            {
                if (int.TryParse(value, out int intValue) && _expertRating?.Budget != intValue)
                {
                    _expertRating?.Budget = intValue;
                    UpdateRatingScore();
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            CurrentUser = AuthService.CurrentUser;

            if (CurrentUser?.Role == RoleType.Expert || CurrentUser?.Role == RoleType.Admin)
                _expertRating = new RatingRequest();

            CurrentIdea = await IdeasService.GetIdeaByIdAsync(IdeaId);
            IdeaSkills = await IdeasService.GetAllIdeaSkillsAsync(IdeaId);
            ideaData = GetIdeaData(CurrentIdea, IdeaSkills);
            IdeaRatings = await IdeasService.GetIdeaRatingsAsync(IdeaId);
            IdeaComments = await IdeasService.GetIdeasCommentsAsync(IdeaId);

            isLoading = false;
        }

        private static List<IdeaModalItem> GetIdeaData(Idea? idea, List<Skill> skills) => [
            new IdeaModalItem
            {
                Title = "Проблема",
                Data = idea?.Problem
            },
            new IdeaModalItem
            {
                Title = "Предлагаемое решение",
                Data = idea?.Solution
            },
            new IdeaModalItem
            {
                Title = "Ожидаемый результат",
                Data = idea?.Result
            },
            new IdeaModalItem
            {
                Title = "Описание необходимых ресурсов для реализации",
                Data = idea?.Description
            },
            new IdeaModalItem
            {
                Title = "Стек технологий",
                Data = skills
            }
        ];

        private void UpdateRatingScore()
        {
            if (_expertRating is null) return;

            if (
                !_expertRating.MarketValue.HasValue || 
                !_expertRating.Originality.HasValue || 
                !_expertRating.TechnicalRealizability.HasValue ||
                !_expertRating.Suitability.HasValue ||
                !_expertRating.Budget.HasValue
            ) return;

            _expertRating.Rating = Formulas.CalculcateRating(
                [
                    _expertRating.MarketValue.Value, 
                    _expertRating.Originality.Value, 
                    _expertRating.TechnicalRealizability.Value,
                    _expertRating.Suitability.Value,
                    _expertRating.Budget.Value
                ]
            );

            StateHasChanged();
        }

        private void ConfirmRating()
        {
            isRatingConfirming = true;



            isRatingConfirming = false;
        }

        private void SaveRating()
        {
            isRatingSaving = true;


            isRatingSaving = false;
            isRatingSaved = true;
        }

        private bool CheckIdeaButtonsAccess()
        {
            if (CurrentUser?.Role is null) return false;

            var userRole = (RoleType)CurrentUser.Role;

            if (userRole == RoleType.Admin) return true;

            if (
                userRole == RoleType.Initiator && CurrentUser.Id == CurrentIdea?.Initiator.Id && 
                (CurrentIdea?.Status == IdeaStatusType.New || CurrentIdea?.Status == IdeaStatusType.OnEditing)
            ) return true;

            if (
                userRole == RoleType.ProjectOffice &&
                (CurrentIdea?.Status == IdeaStatusType.OnApproval || CurrentIdea?.Status == IdeaStatusType.Confirmed)
            ) return true;

            if (userRole == RoleType.Expert && CurrentIdea?.Status == IdeaStatusType.OnConfirmation) return true;

            return false;
        }

        private async Task UpdateIdeaStatus(IdeaStatusType ideaStatus)
        {
            if (CurrentIdea is null) return;

            await IdeasService.UpdateIdeaStatusAsync(CurrentIdea.Id, ideaStatus);
        }

        private async Task HandleActionMenuClick(TableActionContext context)
        {
            if (context.Action == MenuAction.Delete && context.Item is Comment comment)
            {
                if (await IdeasService.DeleteCommentInIdeaAsync(comment))
                    IdeaComments.Remove(comment);
            }
        }

        private async Task NavigateToCreateIdea()
        {
            ModalService.CloseAll(ModalType.RightSide);
            await NavigationService.NavigateToAsync($"/ideas/create/{CurrentIdea?.Id}");
        }
    }
}
