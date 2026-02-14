using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Services.Modal;
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

        private User? CurrentUser { get; set; } = null;
        private Idea? CurrentIdea { get; set; } = null;
        private List<Skill> IdeaSkills { get; set; } = [];
        private List<Rating> IdeaRatings { get; set; } = [];
        private List<Comment> IdeaComments { get; set; } = [];

        private List<IdeaModalItem> ideaData = [];

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            CurrentUser = AuthService.CurrentUser;
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
