using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.IdeaRatings;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Services.IdeaSkills;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.ShowIdeaModal
{
    public partial class ShowIdeaModal
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private IIdeasService IdeasService { get; set; } = null!;

        [Inject]
        private IIdeaSkillService IdeaSkillService { get; set; } = null!;

        [Inject]
        private IIdeaRatingService IdeaRatingService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public Guid IdeaId { get; set; }

        private bool isLoading = true;

        private User? CurrentUser { get; set; } = null;
        private Idea? CurrentIdea { get; set; } = null;
        private List<Skill> IdeaSkills { get; set; } = [];
        private List<Rating> IdeaRatings { get; set; } = [];

        private List<ShowIdeaModalItem> ideaData = [];

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            CurrentUser = AuthService.CurrentUser;
            CurrentIdea = await IdeasService.GetIdeaByIdAsync(IdeaId);
            IdeaSkills = await IdeaSkillService.GetAllIdeaSkillsAsync(IdeaId);
            ideaData = GetIdeaData(CurrentIdea, IdeaSkills);
            IdeaRatings = await IdeaRatingService.GetIdeaRatingsAsync(IdeaId);

            isLoading = false;
        }

        private static List<ShowIdeaModalItem> GetIdeaData(Idea? idea, List<Skill> skills)
        {
            return
            [
                new ShowIdeaModalItem
                {
                    Title = "Проблема",
                    Data = idea?.Problem
                },
                new ShowIdeaModalItem
                {
                    Title = "Предлагаемое решение",
                    Data = idea?.Solution
                },
                new ShowIdeaModalItem
                {
                    Title = "Ожидаемый результат",
                    Data = idea?.Result
                },
                new ShowIdeaModalItem
                {
                    Title = "Описание необходимых ресурсов для реализации",
                    Data = idea?.Description
                },
                // new ShowIdeaModalItem
                // {
                //     Title = "Стек технологий",
                //     Data = skills
                // }
            ];
        }

        private string GetCollapseStyle(ShowIdeaModalItem item)
        {
            if (!item.IsExpanded)
                return "height: 0px; overflow: hidden; transition: height 0.35s ease;";

            return $"overflow: hidden; transition: height 0.35s ease;";
        }
    }
}
