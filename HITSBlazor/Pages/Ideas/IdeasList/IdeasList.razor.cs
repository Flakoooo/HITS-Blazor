using HITSBlazor.Components.ShowIdeaModal;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HITSBlazor.Pages.Ideas.IdeasList
{
    [Authorize]
    [Route("/ideas/list")]
    [Route("/ideas/list/{IdeaId}")]
    public partial class IdeasList
    {
        [Inject]
        private IIdeasService IdeasService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        [Parameter]
        public Guid? IdeaId { get; set; }

        private List<Idea> _ideas = [];

        private bool showCreateIdeaButton = true;

        private readonly string tableDataClass = "py-3 col align-self-center";
        private readonly string tableDataContentClass = "flex-wrap d-flex gap-1";
        private readonly string tableDataContentCenterClass = "justify-content-center align-items-center text-center flex-wrap d-flex gap-1";

        private Guid? IdeaMenuId { get; set; }

        private readonly List<IdeaStatusType> AllStatuses =
        [
            IdeaStatusType.New,
            IdeaStatusType.OnEditing,
            IdeaStatusType.OnApproval,
            IdeaStatusType.OnConfirmation,
            IdeaStatusType.Confirmed,
            IdeaStatusType.OnMarket
        ];

        private HashSet<IdeaStatusType> SelectedStatuses { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _ideas = await IdeasService.GetAllIdeasAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (IdeaId is not null)
                ShowIdea((Guid)IdeaId);
        }

        private static string GetRatingStyle(double? rating) => rating switch
        {
            < 3.0 => "text-danger",
            >= 3.0 and < 4.0 => "text-warning",
            >= 4.0 => "text-success",
            _ => string.Empty
        };

        private async Task OnStatusChanged(IdeaStatusType status, bool isChecked)
        {
            if (isChecked)
                SelectedStatuses.Add(status);
            else
                SelectedStatuses.Remove(status);

            if (SelectedStatuses.Count > 0)
                _ideas = await IdeasService.GetIdeasByStatusAsync([.. SelectedStatuses]);
            else
                _ideas = await IdeasService.GetAllIdeasAsync();
        }

        private async void ResetFilters()
        {
            SelectedStatuses.Clear();
            _ideas = await IdeasService.GetAllIdeasAsync();
        }

        private void ShowIdea(Guid ideaId)
        {
            var modalParameters = new Dictionary<string, object>
            {
                { "IdeaId", ideaId }
            };
            ModalService.Show<ShowIdeaModal>(type: ModalType.RightSide, parameters: modalParameters);
        }
    }
}
