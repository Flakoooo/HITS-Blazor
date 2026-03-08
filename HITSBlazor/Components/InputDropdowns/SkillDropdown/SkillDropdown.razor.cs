using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Services.Skills;
using HITSBlazor.Utils.EnumTranslators;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace HITSBlazor.Components.InputDropdowns.SkillDropdown
{
    public partial class SkillDropdown : IAsyncDisposable
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        [Inject]
        private ISkillService SkillService { get; set; } = null!;

        [Parameter] 
        public SkillType SkillType { get; set; }

        [Parameter] 
        public HashSet<Skill> SelectedSkills { get; set; } = [];

        [Parameter] 
        public EventCallback<HashSet<Skill>> SelectedSkillsChanged { get; set; }

        private ElementReference inputRef;
        private DotNetObjectReference<SkillDropdown>? dotNetHelper;

        private bool _isOpen = false;
        private bool _skillCreateAllowed = false;
        private string _searchText = string.Empty;

        private List<Skill> _skills = [];

        protected override async Task OnInitializedAsync()
        {
            await LoadSkillsAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetHelper = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("listDropdown.registerClickOutside",
                    inputRef, dotNetHelper);
            }
        }

        private async Task LoadSkillsAsync()
        {
            _skills = await SkillService.GetSkillsAsync(
                searchText: _searchText,
                skillType: SkillType
            );
            StateHasChanged();
        }

        private string GetPlaceholder()
            => $"{SkillType.GetTranslation()}{(SelectedSkills.Count > 0 ? $" ({SelectedSkills.Count})" : "")}";

        private async Task OpenDropdown()
        {
            if (!_isOpen) _isOpen = true;
        }

        private void ToggleDropdown() => _isOpen = !_isOpen;

        private async Task CreateNewUnconfirmedSkill()
        {
            var newSkill = await SkillService.CreateNewSkillAsync(_searchText, SkillType, false);
            if (newSkill is null) return;

            SelectedSkills.Add(newSkill);
        }

        [JSInvokable]
        public void CloseDropdown()
        {
            _isOpen = false;
            InvokeAsync(StateHasChanged);
        }

        private async Task OnSearch(ChangeEventArgs e)
        {
            _searchText = (e.Value?.ToString() ?? "").Trim();

            await LoadSkillsAsync();

            _skillCreateAllowed = !string.IsNullOrWhiteSpace(_searchText) 
                && !_skills.Any(s => s.Name.Equals(_searchText, StringComparison.CurrentCultureIgnoreCase));
        }

        private void OnInputKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
                _isOpen = false;
        }

        private async Task OnItemClick(Skill skill)
        {
            if (!SelectedSkills.Remove(skill))
                SelectedSkills.Add(skill);

            await SelectedSkillsChanged.InvokeAsync(SelectedSkills);
        }

        private async Task OnCheckboxChange(Skill skill, bool isChecked)
        {
            if (isChecked)
                SelectedSkills.Add(skill);
            else
                SelectedSkills.Remove(skill);

            await SelectedSkillsChanged.InvokeAsync(SelectedSkills);
        }

        private bool IsSkillSelected(Skill skill) => SelectedSkills.Contains(skill);

        private async Task RemoveSkill(Skill skill)
        {
            SelectedSkills.Remove(skill);
            await SelectedSkillsChanged.InvokeAsync(SelectedSkills);
        }

        public async ValueTask DisposeAsync()
        {
            if (dotNetHelper != null)
            {
                await JSRuntime.InvokeVoidAsync("listDropdown.unregisterClickOutside", inputRef);
                dotNetHelper.Dispose();
            }
        }
    }
}
