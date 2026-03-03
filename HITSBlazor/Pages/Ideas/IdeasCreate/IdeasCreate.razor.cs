using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Companies;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Services.Skills;
using HITSBlazor.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Ideas.IdeasCreate
{
    [Authorize]
    [Route("/ideas/create")]
    [Route("/ideas/create/{IdeaId}")]
    public partial class IdeasCreate
    {
        [Inject]
        private ISkillService SkillService { get; set; } = null!;

        [Inject]
        private ICompanyService CompanyService { get; set; } = null!;

        [Inject]
        private IIdeasService IdeasService { get; set; } = null!;

        [Inject]
        private NavigationService Navigation { get; set; } = null!;

        [Parameter]
        public string IdeaId { get; set; } = string.Empty;

        private bool _isLoading = true;

        private IdeasCreateModel _ideasCreateModel = new();

        private List<Skill> _languageSkills = [];
        private List<Skill> _frameworkSkills = [];
        private List<Skill> _databaseSkills = [];
        private List<Skill> _devopsSkills = [];

        private HashSet<Skill> _selectedLanguageSkills = [];
        private HashSet<Skill> _selectedFrameworkSkills = [];
        private HashSet<Skill> _selectedDatabaseSkills = [];
        private HashSet<Skill> _selectedDevopsSkills = [];

        private List<Company> _companies = [];

        private Company? SelectedCompany { get; set; } = null;
        private User? SelectedContactPerson { get; set; } = null;


        private string SuitabilityScore { 
            get => _ideasCreateModel.Suitability.ToString(); 
            set 
            {
                if (int.TryParse(value, out int intValue) && _ideasCreateModel.Suitability != intValue)
                {
                    _ideasCreateModel.Suitability = intValue;
                    UpdatePreAssessmentScore();
                }
            }
        }


        private string BudgetScore { 
            get => _ideasCreateModel.Budget.ToString();
            set
            {
                if (int.TryParse(value, out int intValue) && _ideasCreateModel.Budget != intValue)
                {
                    _ideasCreateModel.Budget = intValue;
                    UpdatePreAssessmentScore();
                }
            }
        }

        private double? _preAssessmentScore = null;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            _languageSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Language);
            _frameworkSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Framework);
            _databaseSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Database);
            _devopsSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Devops);

            ServiceResponse<List<Company>> companies = await CompanyService.GetAllCompanies();
            if (companies.IsSuccess)
                _companies = companies.Response ?? [];

            _isLoading = false;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!Guid.TryParse(IdeaId, out Guid guid)) return;

            var idea = await IdeasService.GetIdeaByIdAsync(guid);
            if (idea is null) return;

            _ideasCreateModel = new()
            {
                Name = idea.Name,
                Problem = idea.Problem,
                Description = idea.Description,
                Solution = idea.Solution,
                Result = idea.Result,
                Status = idea.Status,
                MaxTeamSize = idea.MaxTeamSize,
                MinTeamSize = idea.MinTeamSize,
                Customer = idea.Customer,
                ContactPerson = idea.ContactPerson
            };

            SelectedCompany = _companies.FirstOrDefault(c => c.Name.Equals(idea.Customer));
            if (SelectedCompany != null)
            {
                SelectedContactPerson = SelectedCompany.Users.FirstOrDefault(
                    u => u.FullName.Equals(idea.ContactPerson)
                );
            }

            SuitabilityScore = idea.Suitability.ToString();
            BudgetScore = idea.Budget.ToString();

            var ideaSkills = await IdeasService.GetAllIdeaSkillsAsync(guid);
            _selectedLanguageSkills = [.. ideaSkills.Where(s => s.Type == SkillType.Language)];
            _selectedFrameworkSkills = [.. ideaSkills.Where(s => s.Type == SkillType.Framework)];
            _selectedDatabaseSkills = [.. ideaSkills.Where(s => s.Type == SkillType.Database)];
            _selectedDevopsSkills = [.. ideaSkills.Where(s => s.Type == SkillType.Devops)];
        }

        private async Task<List<Skill>> SearchSkillsAsync(SkillType skillType, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return skillType switch
                {
                    SkillType.Language => _languageSkills,
                    SkillType.Framework => _frameworkSkills,
                    SkillType.Database => _databaseSkills,
                    SkillType.Devops => _devopsSkills,
                    _ => []
                };
            }

            return await SkillService.GetSkillByTypeAndByNameAsync(skillType, searchText);
        }

        private async Task<List<Company>> SearchCompaniesAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return _companies;

            return await CompanyService.GetCompaniesByName(searchText);
        }

        private async Task<List<User>> SearchContactPersonsAsync(string searchText)
        {
            if (SelectedCompany is not null)
            {
                if (string.IsNullOrWhiteSpace(searchText))
                    return SelectedCompany.Users;

                return [.. SelectedCompany.Users.Where(u => 
                    u.FullName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                ];
            }
            return [];
        }

        private void UpdatePreAssessmentScore()
        {
            _preAssessmentScore = (int.TryParse(SuitabilityScore, out int s) && int.TryParse(BudgetScore, out int b))
                ? Formulas.CalculcateRating([s, b])
                : null;
            StateHasChanged();
        }

        private async Task CreateIdea(IdeaStatusType ideaStatusType)
        {
            if (SelectedCompany is not null && SelectedContactPerson is not null)
            {
                _ideasCreateModel.Status = ideaStatusType;
                _ideasCreateModel.Customer = SelectedCompany.Name;
                _ideasCreateModel.ContactPerson = SelectedContactPerson.FullName;
                if (await IdeasService.CreateNewIdeaAsync(_ideasCreateModel))
                    await Navigation.NavigateToAsync("ideas/list");
            }
        }

        private async Task UpdateIdea()
        {
            if (!Guid.TryParse(IdeaId, out Guid guid)) return;

            if (await IdeasService.UpdateIdeaAsync(guid, _ideasCreateModel))
                await Navigation.NavigateToAsync("ideas/list");
        }
    }
}
