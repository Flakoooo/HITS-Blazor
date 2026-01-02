using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.IdeasList
{
    [Authorize]
    [Route("/ideas/list")]
    public partial class IdeasList
    {
        [Inject]
        private IIdeasService IdeasService { get; set; } = null!;

        private List<Idea> _ideas = [];

        private bool showCreateIdeaButton = true;

        private readonly string tableDataClass = "py-3 col align-self-center";
        private readonly string tableDataContentClass = "flex-wrap d-flex gap-1";
        private readonly string tableDataContentCenterClass = "justify-content-center align-items-center text-center flex-wrap d-flex gap-1";
        private bool dropdownMenuIsShowed = false;

        private readonly string filterChoiceStyle = "cursor: pointer; display: flex; flex-direction: row; flex-wrap: nowrap; align-items: center; justify-content: space-between; transition: background-color;";

        protected override async Task OnInitializedAsync()
        {
            _ideas = await GetAllIdeasAsync();
        }

        private void ChangeDropdownMenuStatus() => dropdownMenuIsShowed = !dropdownMenuIsShowed;

        private async Task<List<Idea>> GetAllIdeasAsync()
        {
            ServiceResponse<List<Idea>> ideas = await IdeasService.GetAllIdeasAsync();
            return ideas.Response ?? [];
        }

        private static string GetStatusStyle(IdeaStatusType status) => status switch
        {
            IdeaStatusType.NEW => "bg-primary-subtle text-primary",
            IdeaStatusType.ON_EDITING or IdeaStatusType.ON_APPROVAL => "bg-warning-subtle text-warning",
            IdeaStatusType.ON_CONFIRMATION => "bg-danger-subtle text-danger",
            IdeaStatusType.CONFIRMED or IdeaStatusType.ON_MARKET => "bg-success-subtle text-success",
            _ => ""
        };

        private static string GetStatusName(IdeaStatusType status) => status switch
        {
            IdeaStatusType.NEW => "Новая",
            IdeaStatusType.ON_EDITING => "На редактировании",
            IdeaStatusType.ON_APPROVAL => "На согласовании",
            IdeaStatusType.ON_CONFIRMATION => "На утверждении",
            IdeaStatusType.CONFIRMED => "Утверждена",
            IdeaStatusType.ON_MARKET => "Опубликована",
            _ => ""
        };

        private static string GetRatingStyle(double rating) => rating switch
        {
            < 3.0 => "text-danger",
            >= 3.0 and < 4.0 => "text-warning",
            >= 4.0 => "text-success",
            _ => ""
        };
    }
}
