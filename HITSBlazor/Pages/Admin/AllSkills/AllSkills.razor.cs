using ApexCharts;
using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Modals.CenterModals.DeleteModal;
using HITSBlazor.Components.Modals.CenterModals.SkillModal;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Skills;
using HITSBlazor.Utils.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Admin.AllSkills
{
    [Authorize]
    [Route("admin/skills")]
    public partial class AllSkills : IDisposable
    {
        [Inject]
        private ISkillService SkillService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private bool _isLoading = true;

        private string _searchText = string.Empty;

        private bool? IsConfirmed { get; set; }

        private List<Skill> _skills = [];

        private readonly List<TableHeaderItem> _skillsTableHeader =
        [
            new TableHeaderItem { Text = "Название",    ColumnClass = "col-4" },
            new TableHeaderItem { Text = "Категория",   ColumnClass = "col-4" },
            new TableHeaderItem { Text = "Статус",      ColumnClass = "col-4" }
        ];

        private readonly List<EnumViewModel<SkillType>> _filterSkillTypes 
            = [.. Enum.GetValues<SkillType>().Select(s => new EnumViewModel<SkillType>(s))];

        private List<EnumViewModel<SkillType>> AllSkillTypes { get; set; } = [];
        private HashSet<EnumViewModel<SkillType>> SelectedSkillTypes { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            _skills = await SkillService.GetSkillsAsync();
            SkillService.OnSkillsStateChanged += LoadSkillsAsync;
            SkillService.OnSkillsStateUpdated += StateHasChanged;

            _isLoading = false;
        }

        private async Task LoadSkillsAsync()
        {
            _skills = await SkillService.GetSkillsAsync(
                searchText: _searchText,
                confirmed: IsConfirmed,
                skillTypes: [.. SelectedSkillTypes.Select(s => s.Value)]
            );
            StateHasChanged();
        }

        private async Task SeacrhSkill(string value)
        {
            _searchText = value;
            await LoadSkillsAsync();
        }

        private static Dictionary<MenuAction, object> GetTableActions(Skill skill)
        {
            var actions = new Dictionary<MenuAction, object>();

            if (skill.Confirmed)
                actions.Add(MenuAction.Edit, skill);
            else
                actions.Add(MenuAction.Confirm, skill.Id);

            actions.Add(MenuAction.Delete, skill);

            return actions;
        }

        private async Task ResetFilters()
        {
            SelectedSkillTypes.Clear();
            IsConfirmed = null;
            await LoadSkillsAsync();
        }

        private void ShowSkillModal(Skill? skill = null)
        {
            if (skill is not null)
            {
                ModalService.Show<SkillModal>(
                    ModalType.Center,
                    parameters: new Dictionary<string, object> { [nameof(SkillModal.Skill)] = skill }
                );
            }
            else
            {
                ModalService.Show<SkillModal>(ModalType.Center);
            }
        }

        private async Task OnSkillAction(TableActionContext context)
        {
            if (context.Action == MenuAction.Confirm)
            {
                if (context.Item is Guid skillId)
                    await SkillService.ConfirmSkillAsync(skillId);
            }
            else if (context.Action == MenuAction.Edit)
            {
                if (context.Item is Skill skill)
                    ShowSkillModal(skill);
            }
            else if (context.Action == MenuAction.Delete)
            {
                if (context.Item is Skill skill)
                    ModalService.ShowDeleteModal(
                        skill.Name,
                        () => SkillService.DeleteSkillAsync(skill)
                    );
            }
        }

        public void Dispose()
        {
            SkillService.OnSkillsStateChanged -= LoadSkillsAsync;
            SkillService.OnSkillsStateUpdated -= StateHasChanged;
        }
    }
}
