using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Services.Skills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Admin.AllSkills
{
    [Authorize]
    [Route("admin/skills")]
    public partial class AllSkills
    {
        [Inject]
        private ISkillService SkillService { get; set; } = null!;

        private bool _isLoading = true;

        private bool? Temp { get; set; }

        private List<Skill> _skills = [];

        private readonly List<TableHeaderItem> _skillsTableHeader =
        [
            new TableHeaderItem { Text = "Название",    ColumnClass = "col-4" },
            new TableHeaderItem { Text = "Категория",   ColumnClass = "col-4" },
            new TableHeaderItem { Text = "Статус",      ColumnClass = "col-4" }
        ];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            _skills = await SkillService.GetSkillsAsync();

            _isLoading = false;
        }
    }
}
