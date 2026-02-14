using HITSBlazor.Components.ActionMenus.BaseActionMenu;
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
        public string IdeaId { get; set; } = string.Empty;

        private string? _searchText = null;
        private List<Idea> _ideas = [];

        private bool showCreateIdeaButton = true;

        private HashSet<IdeaStatusType> SelectedStatuses { get; set; } = [];

        private async Task LoadIdeasAsync()
        {
            _ideas = await IdeasService.GetIdeasAsync(
                searchText: _searchText,
                statusTypes: SelectedStatuses.Count > 0 ? [.. SelectedStatuses] : null
            );
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadIdeasAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Guid.TryParse(IdeaId, out Guid guid))
                ShowIdea(guid);
        }

        private async Task SearchIdea(string value)
        {
            _searchText = value;
            await LoadIdeasAsync();
        }

        private async Task OnStatusChanged(IdeaStatusType status, bool isChecked)
        {
            if (isChecked)
                SelectedStatuses.Add(status);
            else
                SelectedStatuses.Remove(status);

            await LoadIdeasAsync();
        }

        private async void ResetFilters()
        {
            SelectedStatuses.Clear();
            await LoadIdeasAsync();
        }

        private void ShowIdea(Guid ideaId) => ModalService.ShowIdeaModal(ideaId);

        private async Task OnIdeaAction(TableActionContext context)
        {
            if (context.Action == MenuAction.View)
            {
                if (context.Item is Guid guid)
                    ShowIdea(guid);
            }
            else if (context.Action == MenuAction.Edit)
            {
                if (context.Item is Guid guid)
                    await NavigationService.NavigateToAsync($"/ideas/create/{guid}");
            }
            else if (context.Action == MenuAction.Delete)
            {
                if (context.Item is not Idea idea || !await IdeasService.DeleteIdeaAsync(idea)) 
                    return;

                _ideas.Remove(idea);
            }
        }
    }
}
