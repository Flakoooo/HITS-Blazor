using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Teams;
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

        [Parameter]
        public string TeamId { get; set; } = string.Empty;

        private TeamsCreateModel TeamsCreateModel { get; set; } = new();

        private bool _isLoading = true;
        private string _value = string.Empty;

        private List<User> TeamMembers { get; set; } = [];

        private HashSet<Skill> SelectedLanguageSkills { get; set; } = [];
        private HashSet<Skill> SelectedFrameworkSkills { get; set; } = [];
        private HashSet<Skill> SelectedDatabaseSkills { get; set; } = [];
        private HashSet<Skill> SelectedDevopsSkills { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            var currentUser = AuthService.CurrentUser;
            if (currentUser is not null)
                TeamsCreateModel.Owner = currentUser;

            _isLoading = false;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!Guid.TryParse(TeamId, out Guid guid)) return;

            var team = await TeamsService.GetTeamByIdAsync(guid);
            if (team is null) return;

            TeamsCreateModel = new()
            {
                Name = team.Name,
                Description = team.Description,
                Closed = team.Closed,
            };
        }
    }
}
