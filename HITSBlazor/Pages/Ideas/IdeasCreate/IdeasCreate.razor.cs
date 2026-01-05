using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Companies;
using HITSBlazor.Services.Skills;
using HITSBlazor.Utils;
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

        [Inject]
        private ICompanyService CompanyService { get; set; } = null!;

        private bool isLoading = true;

        private List<Skill> LanguageSkills { get; set; } = [];
        private List<Skill> FrameworkSkills { get; set; } = [];
        private List<Skill> DatabaseSkills { get; set; } = [];
        private List<Skill> DevopsSkills { get; set; } = [];

        private HashSet<Skill> SelectedLanguageSkills { get; set; } = [];
        private HashSet<Skill> SelectedFrameworkSkills { get; set; } = [];
        private HashSet<Skill> SelectedDatabaseSkills { get; set; } = [];
        private HashSet<Skill> SelectedDevopsSkills { get; set; } = [];

        private List<Company> Companies { get; set; } = [];

        private Company? SelectedCompany { get; set; } = null;
        private User? SelectedContactPerson { get; set; } = null;

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            LanguageSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Language);
            FrameworkSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Framework);
            DatabaseSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Database);
            DevopsSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Devops);

            ServiceResponse<List<Company>> companies = await CompanyService.GetAllCompanies();
            if (companies.IsSuccess)
                Companies = companies.Response ?? [];

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

        private async Task<List<Company>> SearchCompaniesAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return Companies;

            return await CompanyService.GetCompaniesByName(searchText);
        }

        private async Task<List<User>> SearchContactPersonsAsync(string searchText)
        {
            if (SelectedCompany is not null)
            {
                if (string.IsNullOrWhiteSpace(searchText))
                    return SelectedCompany.Users;

                // Реализуйте поиск контактных лиц
                return [.. SelectedCompany.Users.Where(u => 
                    $"{u.FirstName} {u.LastName}".Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                ];
            }
            return [];
        }
    }
}
