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

        [Inject]
        private GlobalNotificationService GlobalNotificationService { get; set; } = null!;

        [Parameter]
        public string IdeaId { get; set; } = string.Empty;

        private bool _isLoading = true;

        private Dictionary<string, string> _hints = new Dictionary<string, string>()
        {
            [nameof(IdeasCreateModel.Name)] = "Здесь Вы должны написать название для вашей идеи",
            [nameof(IdeasCreateModel.Problem)] = 
                "Необходимо указать проблему или потребность, которую решает предлагаемая идея. Из описания понятно, в чем нуждается потребитель идеи или от чего страдает.<br/>" +
                "<br/>Пример 1 (хороший): Пользователи соцсетей нуждаются в интересном или полезном контенте, которым могут предоставить HTML5 приложения<br/>" +
                "<br/>Пример 2: (плохой): В сети есть множество приложений, которые реализуют различные помощники и игры для досуга"
            ,
            [nameof(IdeasCreateModel.Description)] = 
                "При наличии специальных требований инициатора к оборудованию, программам, участникам, которые необходимо использовать для выполнения работ по проекту или для развертывания решения, которые он не сможет предоставить и которые требуются от команды.<br/>" +
                "<br/>Например: Наличие MacBook, наличие специальных аккаунтов (gmail, apple и т.п.)"
            ,
            [nameof(IdeasCreateModel.Solution)] = 
                "Обязательно описать вид приложения: библиотека, модуль, Web приложение, Мобильное приложение, Desktop приложение, Web API, База данных и т.п. с указанием платформ (Windows, iOs, MacOS, Android, Linux).<br/>" +
                "<br/>Важно описать решение или указать для команд кандидатов, что решение нужно будет придумать в рамках реализации идеи"
            ,
            [nameof(IdeasCreateModel.Result)] = 
                "Обязательно указать степень законченности решения.<br/>" +
                "Например:<br/>" +
                "✓ Решение подключено к существующей системе <br/>" +
                "✓ Решение реализовано на тестовой системе <br/>" +
                "✓ Решение спроектировано и проверено на прототипе"
            ,
            ["TeamSize"] = 
                "Необходимо указать желаемую численность команды, при наличии физических ограничений, т.е. если предполагается предоставление рабочих мест инициатором. В остальных случаях нужно оставить значение по умолчанию"
            ,
            ["Skills"] = 
                "В стеке технологий необходимо указать минимальный набор технологий, которые должны быть использованы командой для реализации. Избегайте в одной идее конкурирующих между собой технологий. Например нельзя указать одновременно MYSQL и PostgreSQL или нельзя указать одновременно VueJS и ReactJS.<br/>" +
                "<br/>Желательно выбирать технологии, которыми Вы лучше всего владеете на практике и легко сможете дать оценку и консультацию команде студентов." +
                "<br/>Если в списке отсутствует нужная Вам технология, то можно самостоятельно добавить ее - нажать на поле стека и вписать название технологии (для добавления нажать на \"+\")."
        };

        private IdeasCreateModel _ideasCreateModel = new();
        private bool _submitted = false;

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
            get => _ideasCreateModel.Suitability > 0 
                ? _ideasCreateModel.Suitability.ToString() 
                : string.Empty; 
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
            get => _ideasCreateModel.Budget > 0 
                ? _ideasCreateModel.Budget.ToString() 
                : string.Empty;
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

            //TODO: Сервис компаний чекнуть
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

        //TODO: реализовать слегка подробное отображение ошибки валидации в уведомлении
        private async Task CreateIdea(IdeaStatusType ideaStatusType)
        {
            bool isInvalid = false;
            if (string.IsNullOrWhiteSpace(_ideasCreateModel.Name)) isInvalid = true;
            if (string.IsNullOrWhiteSpace(_ideasCreateModel.Problem)) isInvalid = true;
            if (string.IsNullOrWhiteSpace(_ideasCreateModel.Description)) isInvalid = true;
            if (string.IsNullOrWhiteSpace(_ideasCreateModel.Solution)) isInvalid = true;
            if (string.IsNullOrWhiteSpace(_ideasCreateModel.Result)) isInvalid = true;

            if (_ideasCreateModel.MaxTeamSize is > 30 or < 2) isInvalid = true;
            if (_ideasCreateModel.MinTeamSize is > 30 or < 2) isInvalid = true;

            if (_selectedLanguageSkills.Count == 0) isInvalid = true;
            if (_selectedFrameworkSkills.Count == 0) isInvalid = true;
            if (_selectedDatabaseSkills.Count == 0) isInvalid = true;
            if (_selectedDevopsSkills.Count == 0) isInvalid = true;

            if (SelectedCompany is null) isInvalid = true;
            if (SelectedContactPerson is null) isInvalid = true;

            if (_ideasCreateModel.Suitability is > 5 or < 1) isInvalid = true;
            if (_ideasCreateModel.Budget is > 5 or < 1) isInvalid = true;

            if (!isInvalid)
            {
                _ideasCreateModel.Status = ideaStatusType;
                _ideasCreateModel.Customer = SelectedCompany!.Name;
                _ideasCreateModel.ContactPerson = SelectedContactPerson!.FullName;

                var result = await IdeasService.CreateNewIdeaAsync(_ideasCreateModel);
                _submitted = true;

                if (result) await Navigation.NavigateToAsync("ideas/list");
                return;
            }

            GlobalNotificationService.ShowError("Заполните все необходимые поля");
            _submitted = true;
        }

        private async Task UpdateIdea()
        {
            if (!Guid.TryParse(IdeaId, out Guid guid)) return;

            if (await IdeasService.UpdateIdeaAsync(guid, _ideasCreateModel))
                await Navigation.NavigateToAsync("ideas/list");
        }
    }
}
