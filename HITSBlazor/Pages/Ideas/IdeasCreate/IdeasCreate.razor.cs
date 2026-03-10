using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Companies;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Services.Skills;
using HITSBlazor.Utils.Models;
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

        private readonly Dictionary<string, string> _hints = new()
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

        private IdeasCreateModel IdeasCreateModel { get; set; } = new();
        private bool _isLoading = true;
        private bool _submitted = false;

        private HashSet<Skill> SelectedLanguageSkills { get; set; } = [];
        private HashSet<Skill> SelectedFrameworkSkills { get; set; } = [];
        private HashSet<Skill> SelectedDatabaseSkills { get; set; } = [];
        private HashSet<Skill> SelectedDevopsSkills { get; set; } = [];

        private List<Company> Companies { get; set; } = [];

        private Company? SelectedCompany { get; set; } = null;
        private User? SelectedContactPerson { get; set; } = null;


        private string SuitabilityScore { 
            get => IdeasCreateModel.Suitability > 0 
                ? IdeasCreateModel.Suitability.ToString() 
                : string.Empty; 
            set 
            {
                if (int.TryParse(value, out int intValue) && IdeasCreateModel.Suitability != intValue)
                {
                    IdeasCreateModel.Suitability = intValue;
                    UpdatePreAssessmentScore();
                }
            }
        }


        private string BudgetScore { 
            get => IdeasCreateModel.Budget > 0 
                ? IdeasCreateModel.Budget.ToString() 
                : string.Empty;
            set
            {
                if (int.TryParse(value, out int intValue) && IdeasCreateModel.Budget != intValue)
                {
                    IdeasCreateModel.Budget = intValue;
                    UpdatePreAssessmentScore();
                }
            }
        }

        private double? _preAssessmentScore = null;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            Companies = await CompanyService.GetCompaniesAsync();

            _isLoading = false;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!Guid.TryParse(IdeaId, out Guid guid)) return;

            var idea = await IdeasService.GetIdeaByIdAsync(guid);
            if (idea is null) return;

            IdeasCreateModel = new()
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

            SelectedCompany = await CompanyService.GetCompanyByNameAsync(idea.Customer);
            if (SelectedCompany != null)
            {
                SelectedContactPerson = SelectedCompany.Users.FirstOrDefault(
                    u => u.FullName.Equals(idea.ContactPerson)
                );
            }

            SuitabilityScore = idea.Suitability.ToString();
            BudgetScore = idea.Budget.ToString();

            var ideaSkills = await IdeasService.GetAllIdeaSkillsAsync(guid);
            SelectedLanguageSkills = [.. ideaSkills.Where(s => s.Type == SkillType.Language)];
            SelectedFrameworkSkills = [.. ideaSkills.Where(s => s.Type == SkillType.Framework)];
            SelectedDatabaseSkills = [.. ideaSkills.Where(s => s.Type == SkillType.Database)];
            SelectedDevopsSkills = [.. ideaSkills.Where(s => s.Type == SkillType.Devops)];
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
            if (ideaStatusType == IdeaStatusType.OnConfirmation)
            {
                bool isInvalid = false;
                if (string.IsNullOrWhiteSpace(IdeasCreateModel.Name)) isInvalid = true;
                if (string.IsNullOrWhiteSpace(IdeasCreateModel.Problem)) isInvalid = true;
                if (string.IsNullOrWhiteSpace(IdeasCreateModel.Description)) isInvalid = true;
                if (string.IsNullOrWhiteSpace(IdeasCreateModel.Solution)) isInvalid = true;
                if (string.IsNullOrWhiteSpace(IdeasCreateModel.Result)) isInvalid = true;

                if (IdeasCreateModel.MaxTeamSize is > 30 or < 2) isInvalid = true;
                if (IdeasCreateModel.MinTeamSize is > 30 or < 2) isInvalid = true;

                if (SelectedCompany is null) isInvalid = true;
                if (SelectedContactPerson is null) isInvalid = true;

                if (IdeasCreateModel.Suitability is > 5 or < 1) isInvalid = true;
                if (IdeasCreateModel.Budget is > 5 or < 1) isInvalid = true;

                if (isInvalid)
                {
                    GlobalNotificationService.ShowError("Заполните все необходимые поля");
                    _submitted = true;
                    return;
                }
            }

            IdeasCreateModel.Status = ideaStatusType;
            IdeasCreateModel.Customer = SelectedCompany!.Name;
            IdeasCreateModel.ContactPerson = SelectedContactPerson!.FullName;

            var result = await IdeasService.CreateNewIdeaAsync(IdeasCreateModel);
            _submitted = true;

            if (result is null) return;

            await IdeasService.CreateOrUpdateIdeasSkills(
                result.Id, 
                [
                    .. SelectedLanguageSkills,
                    .. SelectedFrameworkSkills,
                    .. SelectedDatabaseSkills,
                    .. SelectedDevopsSkills
                ]
            );
            await Navigation.NavigateToAsync("ideas/list");
        }

        private async Task UpdateIdea()
        {
            if (!Guid.TryParse(IdeaId, out Guid guid)) return;

            if (await IdeasService.UpdateIdeaAsync(guid, IdeasCreateModel))
                await Navigation.NavigateToAsync("ideas/list");
        }
    }
}
