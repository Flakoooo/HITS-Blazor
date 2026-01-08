using HITSBlazor.Components.SelectActiveRoleModal;
using HITSBlazor.Components.ShowIdeaModal;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Services.Modal;
using HITSBlazor.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

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

        [Parameter]
        public Guid? IdeaId { get; set; }

        private List<Idea> _ideas = [];

        private bool showCreateIdeaButton = true;

        private readonly string tableDataClass = "py-3 col align-self-center";
        private readonly string tableDataContentClass = "flex-wrap d-flex gap-1";
        private readonly string tableDataContentCenterClass = "justify-content-center align-items-center text-center flex-wrap d-flex gap-1";
        private bool dropdownMenuIsShowed = false;

        private readonly string filterChoiceStyle = "cursor: pointer; display: flex; flex-direction: row; flex-wrap: nowrap; align-items: center; justify-content: space-between; transition: background-color;";

        private readonly List<StatusOption> AllStatuses =
        [
            new() { Value = IdeaStatusType.New,             Text = "Новая"              },
            new() { Value = IdeaStatusType.OnEditing,       Text = "На редактировании"  },
            new() { Value = IdeaStatusType.OnApproval,      Text = "На согласовании"    },
            new() { Value = IdeaStatusType.OnConfirmation,  Text = "На утверждении"     },
            new() { Value = IdeaStatusType.Confirmed,       Text = "Утверждена"         },
            new() { Value = IdeaStatusType.OnMarket,        Text = "Опубликована"       }
        ];

        private HashSet<IdeaStatusType> SelectedStatuses { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _ideas = await GetAllIdeasAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (IdeaId is not null)
                ShowIdea((Guid)IdeaId);
        }

        private void ChangeDropdownMenuStatus() => dropdownMenuIsShowed = !dropdownMenuIsShowed;

        private async Task<List<Idea>> GetAllIdeasAsync()
        {
            ServiceResponse<List<Idea>> ideas = await IdeasService.GetAllIdeasAsync();
            return ideas.Response ?? [];
        }

        private static string GetStatusStyle(IdeaStatusType status) => status switch
        {
            IdeaStatusType.New => "bg-primary-subtle text-primary",
            IdeaStatusType.OnEditing or IdeaStatusType.OnApproval => "bg-warning-subtle text-warning",
            IdeaStatusType.OnConfirmation => "bg-danger-subtle text-danger",
            IdeaStatusType.Confirmed or IdeaStatusType.OnMarket => "bg-success-subtle text-success",
            _ => ""
        };

        private static string GetStatusName(IdeaStatusType status) => status switch
        {
            IdeaStatusType.New => "Новая",
            IdeaStatusType.OnEditing => "На редактировании",
            IdeaStatusType.OnApproval => "На согласовании",
            IdeaStatusType.OnConfirmation => "На утверждении",
            IdeaStatusType.Confirmed => "Утверждена",
            IdeaStatusType.OnMarket => "Опубликована",
            _ => ""
        };

        private static string GetRatingStyle(double rating) => rating switch
        {
            < 3.0 => "text-danger",
            >= 3.0 and < 4.0 => "text-warning",
            >= 4.0 => "text-success",
            _ => ""
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
                _ideas = await GetAllIdeasAsync();
        }

        private async void ResetFilters()
        {
            SelectedStatuses.Clear();
            _ideas = await GetAllIdeasAsync();
        }

        private void ShowIdea(Guid ideaId)
        {
            var modalParameters = new Dictionary<string, object>
            {
                { "IdeaId", ideaId }
            };
            ModalService.Show<ShowIdeaModal>(parameters: modalParameters);
        }
    }
}
