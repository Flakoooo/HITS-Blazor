using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Users.Entities;
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
    public partial class IdeasCreate : IDisposable
    {
        [Inject]
        private ISkillService SkillService { get; set; } = null!;

        [Inject]
        private ICompanyService CompanyService { get; set; } = null!;

        [Inject]
        private IIdeasService IdeasService { get; set; } = null!;

        [Parameter]
        public string IdeaId { get; set; } = string.Empty;

        private bool _isLoading = true;
        private int _dotCount = 0;
        private Timer? _animationTimer;

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


        private string _suitabilityScore = string.Empty;
        private string SuitabilityScore { 
            get => _suitabilityScore; 
            set 
            {
                if (_suitabilityScore != value)
                {
                    _suitabilityScore = value;
                    if (int.TryParse(value, out int suitability))
                        _ideasCreateModel.Suitability = suitability;
                    
                    if (!string.IsNullOrWhiteSpace(BudgetScore))
                        StopAnimation();
                }
            }
        }


        private string _budgetScore = string.Empty;
        private string BudgetScore { 
            get => _budgetScore;
            set
            {
                if (_budgetScore != value)
                {
                    _budgetScore = value;
                    if (int.TryParse(value, out int budget))
                        _ideasCreateModel.Budget = budget;

                    if (!string.IsNullOrWhiteSpace(SuitabilityScore))
                        StopAnimation();
                }
            }
        }

        private double PreAssessmentScore => 
            (int.TryParse(SuitabilityScore, out int s) && int.TryParse(BudgetScore, out int b)) 
                ? Math.Round((s + b) / 2.0, 2)
                : 0.0;

        private string PreAssessmentText => 
            _animationTimer != null
                ? $"Предварительная оценка: вычисление{new string('.', _dotCount)}"
                : $"Предварительная оценка: {PreAssessmentScore:F2}";

        private string PreAssessmentStyle
        {
            get
            {
                if (_animationTimer != null || PreAssessmentScore == 0)
                    return string.Empty;

                var score = PreAssessmentScore;
                var width = score * 20;
                var color = score switch
                {
                    <= 3.0 => "rgb(220, 53, 69)",
                    < 4.0 => "rgb(253, 126, 20)",
                    < 5.0 => "rgb(255, 193, 7)",
                    >= 5.0 => "rgb(25, 135, 84)",
                    _ => "rgb(220, 53, 69)"
                };

                return $"width: {width}%; background-color: {color};";
            }
        }

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            StartDotAnimation();

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

        private void OnSuitabilityChanged(ChangeEventArgs e) 
            => SuitabilityScore = e.Value?.ToString() ?? "";

        private void OnBudgetChanged(ChangeEventArgs e) 
            => BudgetScore = e.Value?.ToString() ?? "";

        private async Task CreateIdea(IdeaStatusType ideaStatusType)
        {
            if (SelectedCompany is not null && SelectedContactPerson is not null)
            {
                _ideasCreateModel.Status = ideaStatusType;
                _ideasCreateModel.Customer = SelectedCompany.Name;
                _ideasCreateModel.ContactPerson = SelectedContactPerson.FullName;
                await IdeasService.CreateNewIdeaAsync(_ideasCreateModel);
            }
        }

        private void StartDotAnimation()
        {
            _animationTimer?.Dispose();

            _animationTimer = new Timer(_ =>
            {
                _dotCount = (_dotCount + 1) % 4;
                InvokeAsync(StateHasChanged);
            }, null, 0, 300);
        }

        private void StopAnimation()
        {
            _animationTimer?.Dispose();
            _animationTimer = null;
        }

        public void Dispose()
        {
            StopAnimation();
        }
    }
}
