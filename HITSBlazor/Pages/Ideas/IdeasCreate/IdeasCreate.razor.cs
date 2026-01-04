using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Services.Skills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;

namespace HITSBlazor.Pages.Ideas.IdeasCreate
{
    [Authorize]
    [Route("/ideas/create")]
    public partial class IdeasCreate
    {
        [Inject]
        private ISkillService SkillService { get; set; } = null!;

        private List<Skill> LanguageSkills { get; set; } = [];
        private List<Skill> FrameworkSkills { get; set; } = [];
        private List<Skill> DatabaseSkills { get; set; } = [];
        private List<Skill> DevopsSkills { get; set; } = [];

        private bool showLanguageSkills = false;
        private bool showFrameworkSkills = false;
        private bool showDatabaseSkills = false;
        private bool showDevopsSkills = false;

        private HashSet<Skill> SelectedLanguageSkills { get; set; } = [];
        private HashSet<Skill> SelectedFrameworkSkills { get; set; } = [];
        private HashSet<Skill> SelectedDatabaseSkills { get; set; } = [];
        private HashSet<Skill> SelectedDevopsSkills { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            LanguageSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Language);
            FrameworkSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Framework);
            DatabaseSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Database);
            DevopsSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Devops);
        }

        private async Task<bool> ShowSkills(SkillType skillType) => skillType switch
        {
            SkillType.Language => showLanguageSkills = !showLanguageSkills,
            SkillType.Framework => showFrameworkSkills = !showFrameworkSkills,
            SkillType.Database => showDatabaseSkills = !showDatabaseSkills,
            SkillType.Devops => showDevopsSkills = !showDevopsSkills,
            _ => false
        };

        private async Task SearchNameSkill(ChangeEventArgs e, SkillType skillType)
        {
            var value = (e.Value?.ToString() ?? "").Trim();
            if (string.IsNullOrEmpty(value))
                return;

            _ = skillType switch
            {
                SkillType.Language => LanguageSkills = await SkillService.GetSkillByTypeAndByNameAsync(skillType, value),
                SkillType.Framework => FrameworkSkills = await SkillService.GetSkillByTypeAndByNameAsync(skillType, value),
                SkillType.Database => DatabaseSkills = await SkillService.GetSkillByTypeAndByNameAsync(skillType, value),
                SkillType.Devops => DevopsSkills = await SkillService.GetSkillByTypeAndByNameAsync(skillType, value),
                _ => []
            };
        }

        private async void OnSkillSelected(Skill skill, bool isChecked) => _ = skill.Type switch
        {
            SkillType.Language => isChecked ? SelectedLanguageSkills.Add(skill) : SelectedLanguageSkills.Remove(skill),
            SkillType.Framework => isChecked ? SelectedFrameworkSkills.Add(skill) : SelectedFrameworkSkills.Remove(skill),
            SkillType.Database => isChecked ? SelectedDatabaseSkills.Add(skill) : SelectedDatabaseSkills.Remove(skill),
            SkillType.Devops => isChecked ? SelectedDevopsSkills.Add(skill) : SelectedDevopsSkills.Remove(skill),
            _ => false
        };
    }
}
