using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Ideas.IdeasList
{
    [Authorize]
    [Route("/ideas/list")]
    [Route("/ideas/list/{IdeaId}")]
    public partial class IdeasList : IDisposable
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

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

        private HashSet<IdeaStatusType> SelectedStatuses { get; set; } = [];
        private bool _unapprovedIdeasByCurrentUser = false;

        private async Task LoadIdeasAsync()
        {
            IdeasQueryType queryType = IdeasQueryType.All;

            if (AuthService.CurrentUser?.Role is RoleType.Initiator)
                queryType = IdeasQueryType.Initiator;
            else if (_unapprovedIdeasByCurrentUser)
                queryType = IdeasQueryType.OnConfirmation;

            _ideas = await IdeasService.GetIdeasAsync(
                    queryType,
                    searchText: _searchText,
                    statusTypes: SelectedStatuses
                );
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            IdeasService.OnIdeasStateChanged += UpdateUIState;
            ModalService.OnCloseSideModalContainer += IdeaModalHasClosed;

            var currentUser = AuthService.CurrentUser;
            if (currentUser?.Role == RoleType.Member)
            {
                SelectedStatuses.Add(IdeaStatusType.Confirmed);
                SelectedStatuses.Add(IdeaStatusType.OnMarket);
            }
            else if (currentUser?.Role == RoleType.ProjectOffice)
            {
                SelectedStatuses.Add(IdeaStatusType.OnApproval);
                SelectedStatuses.Add(IdeaStatusType.Confirmed);
            }

            await LoadIdeasAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Guid.TryParse(IdeaId, out Guid guid))
                ModalService.ShowIdeaModal(guid);
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

        private async Task FindOnConfirmation(bool isChecked)
        {
            _unapprovedIdeasByCurrentUser = isChecked;
            await LoadIdeasAsync();
        }

        private async Task ResetFilters()
        {
            SelectedStatuses.Clear();
            await LoadIdeasAsync();
        }

        private async Task ShowIdea(Guid ideaId)
            => await NavigationService.NavigateToAsync($"/ideas/list/{ideaId}");

        private async Task OnIdeaAction(TableActionContext context)
        {
            if (context.Action == MenuAction.View)
            {
                if (context.Item is Guid guid)
                    await ShowIdea(guid);
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

        private void UpdateUIState() => StateHasChanged();

        private async void IdeaModalHasClosed() 
            => await NavigationService.NavigateToAsync($"/ideas/list");

        public void Dispose()
        {
            IdeasService.OnIdeasStateChanged -= UpdateUIState;
            ModalService.OnCloseSideModalContainer -= IdeaModalHasClosed;
        }
    }
}
