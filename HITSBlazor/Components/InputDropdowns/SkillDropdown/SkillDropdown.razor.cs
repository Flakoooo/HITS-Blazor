using HITSBlazor.Models.Common.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HITSBlazor.Components.InputDropdowns.SkillDropdown
{
    public partial class SkillDropdown
    {
        [Parameter]
        public List<Skill> AllSkills { get; set; } = null!;

        [Parameter]
        public string Placeholder { get; set; } = string.Empty;

        [Parameter] 
        public HashSet<Skill> SelectedSkills { get; set; } = [];

        [Parameter] 
        public EventCallback<HashSet<Skill>> SelectedSkillsChanged { get; set; }

        [Parameter]
        public EventCallback<string> ValueMustCreated { get; set; }

        private async Task CreateNewSkill(string name) 
            => await ValueMustCreated.InvokeAsync(name);

        private async Task RemoveSkill(Skill skill)
        {
            if (SelectedSkills.Remove(skill))
                await SelectedSkillsChanged.InvokeAsync(SelectedSkills);
        }
    }
}
