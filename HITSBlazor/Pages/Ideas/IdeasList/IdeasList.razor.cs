using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Services.Modal;
using HITSBlazor.Utils.Models;
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

        private bool _isLoading = true;

        private string? _searchText = null;
        private List<Idea> _ideas = [];

        private readonly List<EnumViewModel<IdeaStatusType>> _filterIdeaStatus
            = [.. Enum.GetValues<IdeaStatusType>().Select(s => new EnumViewModel<IdeaStatusType>(s))];

        private HashSet<EnumViewModel<IdeaStatusType>> SelectedStatuses { get; set; } = [];
        private bool _unapprovedIdeasByCurrentUser = false;

        private async Task LoadIdeasAsync()
        {
            IdeasQueryType queryType = IdeasQueryType.All;

            if (AuthService.CurrentUser?.Role is RoleType.Initiator)
                queryType = IdeasQueryType.Initiator;
            else if (_unapprovedIdeasByCurrentUser)
                queryType = IdeasQueryType.OnConfirmation;

            _ideas = await IdeasService.GetIdeasAsync(
                    queryType: queryType,
                    searchText: _searchText,
                    statusTypes: [.. SelectedStatuses.Select(s => s.Value)]
                );
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            AuthService.OnActiveRoleChanged += UserRoleHasChanged;
            IdeasService.OnIdeasStateChanged += StateHasChanged;
            ModalService.OnCloseSideModalContainer += IdeaModalHasClosed;

            var currentUser = AuthService.CurrentUser;
            if (currentUser?.Role == RoleType.Member)
            {
                SelectedStatuses.Add(new(IdeaStatusType.Confirmed));
                SelectedStatuses.Add(new(IdeaStatusType.OnMarket));
            }
            else if (currentUser?.Role == RoleType.ProjectOffice)
            {
                SelectedStatuses.Add(new(IdeaStatusType.OnApproval));
                SelectedStatuses.Add(new(IdeaStatusType.Confirmed));
            }

            await LoadIdeasAsync();

            _isLoading = false;
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

        private async void UserRoleHasChanged(RoleType? role)
        {
            if (role is RoleType.Expert)
                _unapprovedIdeasByCurrentUser = true;
            else
                _unapprovedIdeasByCurrentUser = false;

            var currentUser = AuthService.CurrentUser;
            if (currentUser?.Role == RoleType.Member)
            {
                SelectedStatuses.Add(new(IdeaStatusType.Confirmed));
                SelectedStatuses.Add(new(IdeaStatusType.OnMarket));
            }
            else if (currentUser?.Role == RoleType.ProjectOffice)
            {
                SelectedStatuses.Add(new(IdeaStatusType.OnApproval));
                SelectedStatuses.Add(new(IdeaStatusType.Confirmed));
            }
            else SelectedStatuses.Clear();

            await LoadIdeasAsync();
            StateHasChanged();
        }

        private async void IdeaModalHasClosed() 
            => await NavigationService.NavigateToAsync($"/ideas/list");

        public void Dispose()
        {
            AuthService.OnActiveRoleChanged -= UserRoleHasChanged;
            IdeasService.OnIdeasStateChanged -= StateHasChanged;
            ModalService.OnCloseSideModalContainer -= IdeaModalHasClosed;
        }
    }
}
