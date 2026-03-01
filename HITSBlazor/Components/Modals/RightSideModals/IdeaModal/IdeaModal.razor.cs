using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;
using System.Linq;

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

        private Rating? _expertRating = null;

        private string _marketValue = string.Empty;
        private string MarketValue
        {
            get => _marketValue.ToString();
            set 
            {
                if (_marketValue != value)
                {
                    _marketValue = value;
                    if (int.TryParse(value, out int suitability))
                        _expertRating?.MarketValue = suitability;
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            CurrentUser = AuthService.CurrentUser;

            if (CurrentUser?.Role == RoleType.Expert)
                _expertRating = new Rating();

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

        private void ConfirmRating()
        {
            isRatingConfirming = true;



            isRatingConfirming = false;
        }

        private void SaveRating()
        {
            isRatingSaving = true;



            isRatingSaving = false;
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
