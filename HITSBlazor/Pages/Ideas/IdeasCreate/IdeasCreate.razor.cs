using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Services.Skills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Ideas.IdeasCreate
{
    [Authorize]
    [Route("/ideas/create")]
    public partial class IdeasCreate
    {
        [Inject]
        private ISkillService SkillService { get; set; } = null!;

        private bool isLoading = true;

        private List<Skill> LanguageSkills { get; set; } = [];
        private List<Skill> FrameworkSkills { get; set; } = [];
        private List<Skill> DatabaseSkills { get; set; } = [];
        private List<Skill> DevopsSkills { get; set; } = [];

        private HashSet<Skill> SelectedLanguageSkills { get; set; } = [];
        private HashSet<Skill> SelectedFrameworkSkills { get; set; } = [];
        private HashSet<Skill> SelectedDatabaseSkills { get; set; } = [];
        private HashSet<Skill> SelectedDevopsSkills { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            LanguageSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Language);
            FrameworkSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Framework);
            DatabaseSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Database);
            DevopsSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Devops);
            isLoading = false;
        }

        private async Task<List<Skill>> SearchSkillsAsync(SkillType skillType, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return skillType switch
                {
                    SkillType.Language => LanguageSkills,
                    SkillType.Framework => FrameworkSkills,
                    SkillType.Database => DatabaseSkills,
                    SkillType.Devops => DevopsSkills,
                    _ => []
                };
            }

            return await SkillService.GetSkillByTypeAndByNameAsync(skillType, searchText);
        }
    }
}
