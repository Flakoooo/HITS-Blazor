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
        private bool isRatingConfirmed = false;

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

        private double? _expertRatingValue;

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            CurrentUser = AuthService.CurrentUser; 

            CurrentIdea = await IdeasService.GetIdeaByIdAsync(IdeaId);
            IdeaSkills = await IdeasService.GetAllIdeaSkillsAsync(IdeaId);
            ideaData = GetIdeaData(CurrentIdea, IdeaSkills);
            IdeaRatings = await IdeasService.GetIdeaRatingsAsync(IdeaId);
            IdeaComments = await IdeasService.GetIdeasCommentsAsync(IdeaId);

            if (CheckIdeaRatingAccess())
            {
                _expertRating = new RatingRequest();
                var rating = IdeaRatings.FirstOrDefault(r => r.ExpertId == CurrentUser?.Id);
                if (rating is not null)
                {
                    if (rating.IsConfirmed) isRatingConfirmed = true;
                    else
                    {
                        _expertRating.Id = rating.Id;
                        _expertRating.MarketValue = rating.MarketValue;
                        _expertRating.Originality = rating.Originality;
                        _expertRating.TechnicalRealizability = rating.TechnicalRealizability;
                        _expertRating.Suitability = rating.Suitability;
                        _expertRating.Budget = rating.Budget;

                        UpdateRatingScore();
                    }
                }
            }

            if (CurrentIdea is not null)
            {
                await IdeasService.UpdateCheckedIdeaAsync(CurrentIdea.Id);
                CurrentIdea.IsChecked = true;
            }

            isLoading = false;
        }

        private static List<IdeaModalItem> GetIdeaData(Idea? idea, List<Skill> skills) => [
            new IdeaModalItem { Title = "Проблема",                                     Data = idea?.Problem        },
            new IdeaModalItem { Title = "Предлагаемое решение",                         Data = idea?.Solution       },
            new IdeaModalItem { Title = "Ожидаемый результат",                          Data = idea?.Result         },
            new IdeaModalItem { Title = "Описание необходимых ресурсов для реализации", Data = idea?.Description    },
            new IdeaModalItem { Title = "Стек технологий",                              Data = skills               }
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

            _expertRatingValue = Formulas.CalculcateRating(
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

        private async Task ConfirmRating()
        {
            isRatingConfirming = true;

            if (_expertRating is not null && await IdeasService.SendRatingAsync(_expertRating, true))
            {
                isRatingConfirmed = true;
                IdeaRatings.FirstOrDefault(r => r.Id == _expertRating.Id)?.IsConfirmed = true;
                if (IdeaRatings.Count() == IdeaRatings.Count(r => r.IsConfirmed))
                    CurrentIdea?.Status = IdeaStatusType.Confirmed;
            }    

            isRatingConfirming = false;
        }

        private async Task SaveRating()
        {
            isRatingSaving = true;

            if (_expertRating is not null && await IdeasService.SendRatingAsync(_expertRating))
                isRatingSaved = true;

            isRatingSaving = false;
        }

        private bool CheckIdeaRatingAccess()
        {
            if (CurrentUser is null) return false;

            return CurrentIdea?.Status == IdeaStatusType.OnConfirmation 
                && IdeaRatings.Select(r => r.ExpertId).Contains(CurrentUser.Id);
        }

        private bool CheckInitiatorAccess()
        {
            if (CurrentUser?.Role == RoleType.Admin) return true;

            if (CurrentIdea?.Status is IdeaStatusType.New or IdeaStatusType.OnEditing)
            {
                if (CurrentUser?.Role == RoleType.Initiator && CurrentUser?.Id == CurrentIdea?.Initiator.Id) return true;
            }

            return false;
        }

        private bool CheckProjectOfficeAccess()
        {
            if (CurrentUser?.Role == RoleType.Admin) return true;

            if (CurrentIdea?.Status == IdeaStatusType.OnApproval)
            {
                if (CurrentUser?.Id == CurrentIdea.ProjectOffice?.Id) return true;
            }

            return false;
        }

        private async Task UpdateIdeaStatus(IdeaStatusType ideaStatus)
        {
            if (CurrentIdea is null) return;

            if (await IdeasService.UpdateIdeaStatusAsync(CurrentIdea.Id, ideaStatus))
                CurrentIdea.Status = ideaStatus;
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
