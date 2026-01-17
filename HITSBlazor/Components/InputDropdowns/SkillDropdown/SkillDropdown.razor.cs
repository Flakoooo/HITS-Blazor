using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
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

        [Parameter] public SkillType SkillType { get; set; }
        [Parameter] public List<Skill> AllSkills { get; set; } = [];
        [Parameter] public HashSet<Skill> SelectedSkills { get; set; } = [];
        [Parameter] public EventCallback<HashSet<Skill>> SelectedSkillsChanged { get; set; }
        [Parameter] public Func<SkillType, string, Task<List<Skill>>>? SearchFunction { get; set; }

        private ElementReference inputRef;
        private bool IsOpen { get; set; }
        private string searchText = "";
        private List<Skill> FilteredSkills { get; set; } = [];
        private DotNetObjectReference<SkillDropdown>? dotNetHelper;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetHelper = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("dropdownManager.registerClickOutside",
                    inputRef, dotNetHelper);

                FilteredSkills = [.. AllSkills];
            }
        }

        private string GetPlaceholder()
            => $"{SkillType.GetTranslation()}{(SelectedSkills.Count > 0 ? $" ({SelectedSkills.Count})" : "")}";

        private async Task OpenDropdown()
        {
            if (!IsOpen)
                IsOpen = true;
        }

        private void ToggleDropdown() => IsOpen = !IsOpen;

        [JSInvokable]
        public void CloseDropdown()
        {
            IsOpen = false;
            InvokeAsync(StateHasChanged);
        }

        private async Task OnSearch(ChangeEventArgs e)
        {
            searchText = (e.Value?.ToString() ?? "").Trim();

            if (SearchFunction != null)
                FilteredSkills = await SearchFunction(SkillType, searchText);
        }

        private void OnInputKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
                IsOpen = false;
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
                await JSRuntime.InvokeVoidAsync("dropdownManager.unregisterClickOutside", inputRef);
                dotNetHelper.Dispose();
            }
        }
    }
}
