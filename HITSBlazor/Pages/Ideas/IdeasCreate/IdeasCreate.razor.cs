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
    public partial class IdeasCreate : IDisposable
    {
        [Inject]
        private ISkillService SkillService { get; set; } = null!;

        [Inject]
        private ICompanyService CompanyService { get; set; } = null!;

        [Inject]
        private IIdeasService IdeasService { get; set; } = null!;

        private bool isLoading = true;

        private IdeasCreateModel ideasCreateModel = new();

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

        private string? SuitabilityScore { get; set; }
        private string? BudgetScore { get; set; }
        private double PreAssessmentScore { get; set; } = 0.0;

        private string preAssessmentText = "Предварительная оценка:";
        private Timer? animationTimer;
        private int dotCount = 0;
        private string preAssessmentStyle = "";


        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            StartDotAnimation();

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

                return [.. SelectedCompany.Users.Where(u => 
                    $"{u.FirstName} {u.LastName}".Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                ];
            }
            return [];
        }

        private void OnSuitabilityChanged(ChangeEventArgs e)
        {
            string selectedValue = e.Value?.ToString() ?? "";
            Console.WriteLine($"Выбрано: {selectedValue}");

            SuitabilityScore = selectedValue;

            CalculatePreAssessmentScore();
        }

        private void OnBudgetChanged(ChangeEventArgs e)
        {
            string selectedValue = e.Value?.ToString() ?? "";
            Console.WriteLine($"Выбрано: {selectedValue}");

            BudgetScore = selectedValue;

            CalculatePreAssessmentScore();
        }

        private void CalculatePreAssessmentScore()
        {
            if (string.IsNullOrEmpty(SuitabilityScore) && string.IsNullOrEmpty(BudgetScore))
                return;

            if (int.TryParse(SuitabilityScore, out int suitability) && int.TryParse(BudgetScore, out int budget))
            {
                PreAssessmentScore = Math.Round((suitability + budget) / 2.0, 2);

                ideasCreateModel.Suitability = suitability;
                ideasCreateModel.Budget = budget;

                preAssessmentText = $"Предварительная оценка: {PreAssessmentScore}";
                preAssessmentStyle = $"width: {PreAssessmentScore * 20}%; " + PreAssessmentScore switch
                {
                    <= 3.0 => "background-color: rgb(220, 53, 69);",
                    < 4.0 => "background-color: rgb(253, 126, 20);",
                    < 5.0 => "background-color: rgb(255, 193, 7);",
                    >= 5.0 => "background-color: rgb(25, 135, 84);",
                    _ => "background-color: rgb(220, 53, 69);"
                };
                if (animationTimer != null) StopAnimation();
            }
        }

        private async Task CreateIdea(IdeaStatusType ideaStatusType)
        {
            if (SelectedCompany is not null && SelectedContactPerson is not null)
            {
                ideasCreateModel.Status = ideaStatusType;
                ideasCreateModel.Customer = SelectedCompany.Name;
                ideasCreateModel.ContactPerson = $"{SelectedContactPerson.FirstName} {SelectedContactPerson.LastName}";
                await IdeasService.CreateNewIdea(ideasCreateModel);
            }
        }

        private void StartDotAnimation()
        {
            animationTimer?.Dispose();

            animationTimer = new Timer(_ =>
            {
                dotCount = (dotCount + 1) % 4;
                preAssessmentText = $"Предварительная оценка: вычисление{new string('.', dotCount)}";

                InvokeAsync(StateHasChanged);
            }, null, 0, 300);
        }

        private void StopAnimation()
        {
            animationTimer?.Dispose();
            animationTimer = null;
        }

        public void Dispose()
        {
            StopAnimation();
        }
    }
}
