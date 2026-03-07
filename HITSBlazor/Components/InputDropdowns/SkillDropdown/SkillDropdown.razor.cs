using ApexCharts;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Services.Skills;
using HITSBlazor.Utils.EnumTranslators;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace HITSBlazor.Components.InputDropdowns.SkillDropdown
{
    //TODO: Перенести поиск навыков сюда, а также получение навыков
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

        [Parameter]
        public bool NeedValidation { get; set; } = false;

        [Parameter]
        public string? ErrorMessage { get; set; } = " не выбраны";

        private ElementReference inputRef;
        private DotNetObjectReference<SkillDropdown>? dotNetHelper;

        private bool _showError = false;
        private bool _isOpen = false;
        private bool _skillCreateAllowed = false;
        private string _searchText = string.Empty;

        public List<Skill> AllSkills { get; set; } = [];
        private List<Skill> FilteredSkills { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            AllSkills = await SkillService.GetSkillsByTypeAsync(SkillType);
        }

        protected override void OnParametersSet()
        {
            ErrorMessage = $"{SkillType.GetTranslation()} не выбраны";
            _showError = NeedValidation && SelectedSkills.Count == 0;
            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetHelper = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("listDropdown.registerClickOutside",
                    inputRef, dotNetHelper);

                FilteredSkills = [.. AllSkills];
            }
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
            if (!AllSkills.Contains(newSkill)) AllSkills.Add(newSkill);
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

            if (string.IsNullOrWhiteSpace(_searchText))
                FilteredSkills = AllSkills;
            else
                FilteredSkills = await SkillService.GetSkillByTypeAndByNameAsync(SkillType, _searchText);

            _skillCreateAllowed = !string.IsNullOrWhiteSpace(_searchText) 
                && !FilteredSkills.Any(s => s.Name.Equals(_searchText, StringComparison.CurrentCultureIgnoreCase));
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
