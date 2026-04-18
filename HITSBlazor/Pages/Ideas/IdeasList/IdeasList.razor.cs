using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.RightSideModals.SendIdeaOnMarketModal;
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
using Microsoft.JSInterop;

namespace HITSBlazor.Pages.Ideas.IdeasList
{
    [Authorize]
    [Route("/ideas/list")]
    [Route("/ideas/list/{IdeaId}")]
    public partial class IdeasList : IAsyncDisposable
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private IIdeasService IdeasService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject] 
        private IJSRuntime JSRuntime { get; set; } = null!;

        [Inject]
        private GlobalNotificationService NotificationService { get; set; } = null!;

        [Parameter]
        public string IdeaId { get; set; } = string.Empty;

        private bool _isLoading = true;
        private bool _isLoadingMore = false;

        private ElementReference _tableContainer;
        private DotNetObjectReference<IdeasList>? _dotNetHelper;
        private IJSObjectReference? _jsModule;
        private bool _isInitialized = false;

        private string _searchText = string.Empty;

        private readonly List<Idea> _ideas = [];
        private HashSet<Idea> _selectedIdeas = [];
        private int _currentPage = 1;
        private int _totalCount = 0;

        private readonly List<EnumViewModel<IdeaStatusType>> _filterIdeaStatus
            = [.. Enum.GetValues<IdeaStatusType>().Select(s => new EnumViewModel<IdeaStatusType>(s))];

        private HashSet<EnumViewModel<IdeaStatusType>> SelectedStatuses { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            AuthService.OnActiveRoleChanged += UserRoleHasChanged;
            IdeasService.OnIdeasStateChanged += StateHasChanged;
            IdeasService.OnIdeaHasDeleted += IdeaHasDeleted;
            ModalService.OnRightSideModalsUpdated += IdeaModalHasClosed;

            _totalCount = await IdeasService.GetTotalIdeaCount();

            SetFilterByRole(AuthService.CurrentUser?.Role);

            await LoadIdeasAsync();

            _isLoading = false;
            _isInitialized = true;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrWhiteSpace(IdeaId) && Guid.TryParse(IdeaId, out Guid ideaId))
                ModalService.ShowIdeaModal(ideaId);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && _isInitialized)
            {
                _dotNetHelper = DotNetObjectReference.Create(this);
                try
                {
                    _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/infiniteScroll.js");
                    await _jsModule.InvokeVoidAsync("initializeInfiniteScroll", _tableContainer, _dotNetHelper);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to initialize infinite scroll: {ex.Message}");
                }
            }
        }

        [JSInvokable]
        public async Task LoadMoreItems()
        {
            if (_isLoadingMore || _isLoading) return;

            if (_ideas.Count >= _totalCount)
            {
                if (_jsModule is not null)
                    await _jsModule.InvokeVoidAsync("stopInfiniteScroll");

                return;
            }

            await LoadIdeasAsync(append: true);
        }

        private async Task LoadIdeasAsync(bool append = false, int page = 1)
        {
            if (!append)
            {
                _currentPage = page;
                _ideas.Clear();
            }
            else
            {
                _isLoadingMore = true;
            }

            StateHasChanged();

            var newIdeas = await IdeasService.GetIdeasAsync(
                _currentPage,
                searchText: _searchText,
                statusTypes: [.. SelectedStatuses.Select(s => s.Value)]
            );

            if (newIdeas.Count > 0)
            {
                if (append)
                    _ideas.AddRange(newIdeas);
                else
                {
                    _ideas.Clear();
                    _ideas.AddRange(newIdeas);
                }
                ++_currentPage;
            }

            if (!append)
            {
                _totalCount = await IdeasService.GetTotalIdeaCount(
                    searchText: _searchText,
                    statusTypes: [.. SelectedStatuses.Select(s => s.Value)]
                );
            }
            else
            {
                if (_ideas.Count > _totalCount)
                    _totalCount = _ideas.Count;
            }

            _isLoadingMore = false;

            StateHasChanged();
        }

        private void SetFilterByRole(RoleType? activeRole)
        {
            if (activeRole is RoleType.Member)
            {
                SelectedStatuses.Add(new(IdeaStatusType.Confirmed));
                SelectedStatuses.Add(new(IdeaStatusType.OnMarket));
            }
            else if (activeRole is RoleType.ProjectOffice)
            {
                SelectedStatuses.Add(new(IdeaStatusType.OnApproval));
                SelectedStatuses.Add(new(IdeaStatusType.Confirmed));
            }
            else if (activeRole is RoleType.Expert)
            {
                SelectedStatuses.Add(new(IdeaStatusType.OnConfirmation));
            }
            else
            {
                SelectedStatuses.Clear();
            }
        }

        private void SelectIdea(Idea idea)
        {
            if (!_selectedIdeas.Add(idea))
                _selectedIdeas.Remove(idea);
        }

        private async Task SearchIdea(string value)
        {
            Console.WriteLine($"новое {value}");
            _searchText = value;
            _currentPage = 1;
            _totalCount = 0;
            await LoadIdeasAsync();
        }

        private async Task ResetFilters()
        {
            SelectedStatuses.Clear();
            _currentPage = 1;
            _totalCount = 0;
            await LoadIdeasAsync();
        }

        private async Task ShowIdea(Guid ideaId)
        {
            if (AuthService.CurrentUser?.Role is RoleType.Admin)
                await NavigationService.NavigateToAsync($"/ideas/list/{ideaId}");
            else
                ModalService.ShowIdeaModal(ideaId);
        }

        private void ShowSendIdeaOnMarket()
        {
            if (_selectedIdeas.Count == 0)
            {
                NotificationService.ShowError("Выберите хотя бы одну идею со статусом \"Утверждена\"");
                return;
            }

            ModalService.Show<SendIdeaOnMarketModal>(
                ModalType.RightSide,
                parameters: new Dictionary<string, object> { 
                    [nameof(SendIdeaOnMarketModal.IdeaForMarket)] = _selectedIdeas
                }
            );
        }

        private Dictionary<MenuAction, object> GetTableActions(Idea idea)
        {
            var actions = new Dictionary<MenuAction, object> { [MenuAction.View] = idea.Id };
            if (AuthService.CurrentUser?.Id == idea.Initiator.Id || AuthService.CurrentUser?.Role is RoleType.Admin)
            {
                actions.Add(MenuAction.Edit, idea.Id);
                actions.Add(MenuAction.Delete, idea);
            }

            return actions;
        }

        private async Task OnIdeaAction(TableActionContext context)
        {
            if (context.Item is Guid guid)
            {
                if (context.Action == MenuAction.View)
                    await ShowIdea(guid);
                else if (context.Action == MenuAction.Edit)
                    await NavigationService.NavigateToAsync($"/ideas/create/{guid}");
            }
            else if (context.Action == MenuAction.Delete)
            {
                if (context.Item is not Idea idea) return;

                ModalService.ShowConfirmModal(
                    $"Вы действительно хотите удалить {idea.Name}?",
                    () => IdeasService.DeleteIdeaAsync(idea),
                    confirmButtonVariant: ButtonVariant.Danger,
                    confirmButtonText: "Удалить"
                );
            }
        }

        private async void UserRoleHasChanged(RoleType? role)
        {
            SetFilterByRole(AuthService.CurrentUser?.Role);

            await LoadIdeasAsync();

            StateHasChanged();
        }

        private async void IdeaModalHasClosed()
        {
            if (AuthService.CurrentUser?.Role is RoleType.Admin && ModalService.SideModals.Count == 0)
                await NavigationService.NavigateToAsync($"/ideas/list");

            StateHasChanged();
        }

        private async void IdeaHasDeleted(Idea idea)
        {
            _ideas.Remove(idea);
            --_totalCount;
            StateHasChanged();
        }

        public async ValueTask DisposeAsync()
        {
            AuthService.OnActiveRoleChanged -= UserRoleHasChanged;
            IdeasService.OnIdeasStateChanged -= StateHasChanged;
            IdeasService.OnIdeaHasDeleted -= IdeaHasDeleted;
            ModalService.OnRightSideModalsUpdated -= IdeaModalHasClosed;

            _dotNetHelper?.Dispose();

            if (_jsModule != null)
                await _jsModule.DisposeAsync();
        }
    }
}
