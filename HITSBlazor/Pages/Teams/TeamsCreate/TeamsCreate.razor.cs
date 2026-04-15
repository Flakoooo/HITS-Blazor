using ApexCharts;
using HITSBlazor.Components.Skills;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Skills;
using HITSBlazor.Services.Teams;
using HITSBlazor.Services.UserSkills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Teams.TeamsCreate
{
    [Authorize]
    [Route("/teams/create")]
    [Route("/teams/create/{TeamId}")]
    public partial class TeamsCreate
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private ITeamService TeamsService { get; set; } = null!;

        [Inject]
        private ISkillService SkillService { get; set; } = null!;

        [Inject]
        private IUserSkillService UserSkillService { get; set; } = null!;

        [Parameter]
        public string TeamId { get; set; } = string.Empty;

        private TeamsCreateModel TeamsCreateModel { get; set; } = new();

        private bool _isLoading = true;
        private string _value = string.Empty;

        private List<User> TeamMembers { get; set; } = [];

        private HashSet<Skill> TeamSkills { get; set; } = [];

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
            _isLoading = true;

            LanguageSkills = await SkillService.GetSkillsAsync(skillTypes: [SkillType.Language]);
            FrameworkSkills = await SkillService.GetSkillsAsync(skillTypes: [SkillType.Framework]);
            DatabaseSkills = await SkillService.GetSkillsAsync(skillTypes: [SkillType.Database]);
            DevopsSkills = await SkillService.GetSkillsAsync(skillTypes: [SkillType.Devops]);

            if (Guid.TryParse(TeamId, out Guid guid))
            {
                var team = await TeamsService.GetTeamByIdAsync(guid);
                if (team is not null)
                {
                    TeamsCreateModel = new()
                    {
                        Name = team.Name,
                        Description = team.Description,
                        Closed = team.Closed,
                    };

                    _isLoading = false;
                }
            }
            else
            {
                var currentUser = AuthService.CurrentUser;
                if (currentUser is not null)
                    TeamsCreateModel.Owner = currentUser;

                _isLoading = false;
            }
        }

        private async Task CreateNewSkill(string name, SkillType skillType)
        {
            var newSkill = await SkillService.CreateNewSkillAsync(name, skillType, false);
            if (newSkill is null) return;

            if (skillType is SkillType.Language)
            {
                LanguageSkills.Add(newSkill);
                SelectedLanguageSkills.Add(newSkill);
            }
            else if (skillType is SkillType.Framework)
            {
                FrameworkSkills.Add(newSkill);
                SelectedFrameworkSkills.Add(newSkill);
            }
            else if (skillType is SkillType.Database)
            {
                DatabaseSkills.Add(newSkill);
                SelectedDatabaseSkills.Add(newSkill);
            }
            else if (skillType is SkillType.Devops)
            {
                DevopsSkills.Add(newSkill);
                SelectedDevopsSkills.Add(newSkill);
            }
        }

        private async Task UpdateTeamSkills()
        {
            TeamSkills.Clear();
            TeamMembers.ForEach(async m => 
                (await UserSkillService.GetUserSkillsAsync(m.Id)).ForEach(s => 
                    TeamSkills.Add(s)
                )
            );
        }

        private static ApexChartOptions<Skill> GetRadarChartOptions() => new()
        {
            Chart = new Chart
            {
                Type = ChartType.Radar,
                Toolbar = new Toolbar { Show = false }
            },
            Yaxis =
                [
                    new YAxis
                {
                    Min = 0,
                    Max = 1,
                    Labels = new YAxisLabels { Show = false },
                    Show = false
                }
                ],
            Xaxis = new XAxis
            {
                Labels = new XAxisLabels
                {
                    Style = new AxisLabelStyle
                    {
                        Colors = new List<string> { "#a8a8a8" },
                        FontSize = "11px"
                    }
                }
            },
            Stroke = new Stroke { Width = 2 },
            Fill = new Fill
            {
                Opacity = 0.5,
                Type = new List<FillType> { FillType.Solid }
            },
            Markers = new Markers { Size = 4 },
            Legend = new Legend
            {
                Show = true,
                Position = LegendPosition.Bottom
            }
        };
    }
}
