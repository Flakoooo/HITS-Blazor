using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.RightSideModals.SendIdeaOnMarketModal;
using HITSBlazor.Components.Tables.TableComponent;
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
    public partial class IdeasList
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
        private GlobalNotificationService NotificationService { get; set; } = null!;

        [Parameter]
        public string IdeaId { get; set; } = string.Empty;

        private bool _isLoading = true;

        private string _searchText = string.Empty;

        private readonly List<Idea> _ideas = [];
        private readonly HashSet<Idea> _selectedIdeas = [];

        private TableComponent? _tableComponent;

        private readonly List<EnumViewModel<IdeaStatusType>> _filterIdeaStatus
            = [.. Enum.GetValues<IdeaStatusType>().Select(s => new EnumViewModel<IdeaStatusType>(s))];

        private HashSet<EnumViewModel<IdeaStatusType>> SelectedStatuses { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            AuthService.OnActiveRoleChanged += UserRoleHasChanged;
            IdeasService.OnIdeaHasDeleted += IdeaHasDeleted;
            IdeasService.OnIdeaHasOpened += ChangeIdeasCheckStatus;
            IdeasService.OnIdeasStatusHasChanged += ChangeIdeasStatus;
            ModalService.OnRightSideModalsUpdated += IdeaModalHasClosed;

            SetFilterByRole(AuthService.CurrentUser?.Role);

            await LoadIdeasAsync();

            _isLoading = false;

            MarkAsInitialized();
        }

        protected override void OnParametersSet()
        {
            if (!string.IsNullOrWhiteSpace(IdeaId) && Guid.TryParse(IdeaId, out Guid ideaId))
                ModalService.ShowIdeaModal(ideaId);
        }

        protected override async Task AdditionalAfterRenderMethod()
        {
            if (_tableComponent != null)
                _tableContainer = _tableComponent.ScrollContainer;
        }

        protected override int GetCurrentItemsCount() => _ideas.Count;

        protected override async Task OnLoadMoreItemsAsync()
        {
            await LoadIdeasAsync(append: true);
        }

        private async Task LoadIdeasAsync(bool append = false)
        {
            if (!append)
            {
                ResetPagination();
                _ideas.Clear();
            }

            StateHasChanged();

            var listResponse = await IdeasService.GetIdeasAsync(
                _currentPage,
                searchText: _searchText,
                statusTypes: [.. SelectedStatuses.Select(s => s.Value)]
            );

            _totalCount = listResponse.Count;
            if (listResponse.List.Count > 0)
            {
                if (append)
                    _ideas.AddRange(listResponse.List);
                else
                {
                    _ideas.Clear();
                    _ideas.AddRange(listResponse.List);
                }

                IncrementPage();
            }

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
            _searchText = value;
            ResetPagination();
            await LoadIdeasAsync();
        }

        private async Task ResetFilters()
        {
            SelectedStatuses.Clear();
            ResetPagination();
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
                if (context.Action is MenuAction.View)
                    await ShowIdea(guid);
                else if (context.Action is MenuAction.Edit)
                    await NavigationService.NavigateToAsync($"/ideas/create/{guid}");
            }
            else if (context.Action is MenuAction.Delete)
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

        private void IdeaHasDeleted(Idea idea)
        {
            _ideas.Remove(idea);
            --_totalCount;
            StateHasChanged();
        }

        private void ChangeIdeasCheckStatus(Guid ideaId, bool isChecked)
        {
            _ideas.FirstOrDefault(i => i.Id == ideaId)?.IsChecked = isChecked;
        }

        private void ChangeIdeasStatus(Guid ideaId, IdeaStatusType ideaStatus)
        {
            _ideas.FirstOrDefault(i => i.Id == ideaId)?.Status = ideaStatus;
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            AuthService.OnActiveRoleChanged -= UserRoleHasChanged;
            IdeasService.OnIdeaHasDeleted -= IdeaHasDeleted;
            IdeasService.OnIdeaHasOpened -= ChangeIdeasCheckStatus;
            IdeasService.OnIdeasStatusHasChanged -= ChangeIdeasStatus;
            ModalService.OnRightSideModalsUpdated -= IdeaModalHasClosed;

            await ValueTask.CompletedTask;
        }
    }
}
