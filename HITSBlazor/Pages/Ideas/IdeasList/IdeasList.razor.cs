using HITSBlazor.Components.Tables.TableActionMenu;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Services.Modal;
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

        private string? _searchText = null;
        private List<Idea> _ideas = [];

        private bool showCreateIdeaButton = true;

        private HashSet<IdeaStatusType> SelectedStatuses { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _ideas = await IdeasService.GetAllIdeasAsync(true);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (IdeaId is not null)
                ShowIdea((Guid)IdeaId);
        }

        private async Task SearchIdea(string value)
        {
            _searchText = value;
            if (SelectedStatuses.Count > 0)
                _ideas = await IdeasService.GetIdeasByStatusAsync(_searchText, statusTypes: [.. SelectedStatuses]);
            else
                _ideas = await IdeasService.GetAllIdeasAsync(seacrhText: _searchText);
        }

        private async Task OnStatusChanged(IdeaStatusType status, bool isChecked)
        {
            if (isChecked)
                SelectedStatuses.Add(status);
            else
                SelectedStatuses.Remove(status);

            if (SelectedStatuses.Count > 0)
                _ideas = await IdeasService.GetIdeasByStatusAsync(_searchText, statusTypes: [.. SelectedStatuses]);
            else
                _ideas = await IdeasService.GetAllIdeasAsync(seacrhText: _searchText);
        }

        private async void ResetFilters()
        {
            SelectedStatuses.Clear();
            _ideas = await IdeasService.GetAllIdeasAsync(seacrhText: _searchText);
        }

        private void ShowIdea(Guid ideaId) => ModalService.ShowIdeaModal(ideaId);

        private async Task OnIdeaAction(TableActionContext context)
        {
            if (context.Action == TableAction.View)
            {
                if (context.Item is Guid guid)
                    ShowIdea(guid);
            }
            else if (context.Action == TableAction.Edit)
            {
                Console.WriteLine($"Редактирование идеи {context.Item}");
            }
            else 
            {
                Console.WriteLine($"Удаление идеи {context.Item}");
            }
        }
    }
}
