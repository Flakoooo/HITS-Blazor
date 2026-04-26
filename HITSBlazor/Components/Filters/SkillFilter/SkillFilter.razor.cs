using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Skills;
using HITSBlazor.Services.UserSkills;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Filters.SkillFilter
{
    public partial class SkillFilter
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private ISkillService SkillService { get; set; } = null!;

        [Inject]
        private IUserSkillService UserSkillService { get; set; } = null!;

        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public string SeacrhSkillText { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<string> SeacrhSkillTextChanged { get; set; }

        [Parameter]
        public HashSet<Guid> SelectedSkillIds { get; set; } = [];

        [Parameter]
        public EventCallback<HashSet<Guid>> SelectedSkillIdsChanged { get; set; }

        [Parameter]
        public EventCallback OnFilterChanged { get; set; }

        private bool IsOnLoadingNow => _isLoading || IsLoading;

        private bool _isLoading = true;

        private List<Skill> _skills = [];
        private HashSet<Guid> _userSkillIds = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (AuthService.CurrentUser is not null)
                _userSkillIds = (await UserSkillService.GetUserSkillsAsync(AuthService.CurrentUser.Id)).Select(s => s.Id).ToHashSet();

            await LoadSkillsAsync();

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async Task OnParametersSetAsync()
        {
            await LoadSkillsAsync();
        }

        protected override async Task OnLoadMoreItemsAsync()
            => await LoadSkillsAsync(append: true);

        protected override int GetCurrentItemsCount() => _skills.Count;

        private async Task LoadSkillsAsync(bool append = false)
        {
            await LoadDataAsync(
                _skills,
                () => SkillService.GetSkillsAsync(
                    _currentPage,
                    searchText: SeacrhSkillText
                ),
                append: append
            );

            _skills = _skills
                .OrderByDescending(s => _userSkillIds.Contains(s.Id))
                .ThenBy(s => s.Name).ToList();

            StateHasChanged();
        }

        private async Task SearchSkill(string value)
        {
            SeacrhSkillText = value;
            ResetPagination();
            await SeacrhSkillTextChanged.InvokeAsync(value);
        }

        private async Task SelectedSkillsChanged(Guid skillId)
        {
            if (!SelectedSkillIds.Add(skillId))
                SelectedSkillIds.Remove(skillId);

            await SelectedSkillIdsChanged.InvokeAsync(SelectedSkillIds);
            await OnFilterChanged.InvokeAsync();
        }
    }
}
