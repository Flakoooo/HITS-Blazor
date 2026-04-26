using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.CenterModals.SkillModal;
using HITSBlazor.Components.Tables.TableComponent;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Skills;
using HITSBlazor.Utils.Models;
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

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private bool _isLoading = true;

        private TableComponent? _tableComponent;

        private string _searchText = string.Empty;

        private readonly List<ValueViewModel<bool?>> _confirmedFilterValues = 
        [
            new(true, "Утверждена"),
            new(false, "На рассмотрении")
        ];

        private ValueViewModel<bool?>? IsConfirmed { get; set; }

        private readonly List<Skill> _skills = [];

        private readonly List<TableHeaderItem> _skillsTableHeader =
        [
            new TableHeaderItem { Text = "Название",    ColumnClass = "col-4" },
            new TableHeaderItem { Text = "Категория",   ColumnClass = "col-4" },
            new TableHeaderItem { Text = "Статус",      ColumnClass = "col-4" }
        ];

        private readonly List<EnumViewModel<SkillType>> _filterSkillTypes 
            = [.. Enum.GetValues<SkillType>().Select(s => new EnumViewModel<SkillType>(s))];

        private HashSet<EnumViewModel<SkillType>> SelectedSkillTypes { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            SkillService.OnSkillHasCreated += SkillHasCreated;
            SkillService.OnSkillHasUpdated += SkillHasUpdated;
            SkillService.OnSkillHasDeleted += SkillHasDeleted;

            _isLoading = false;

            MarkAsInitialized();
        }

        protected override async Task AdditionalAfterRenderMethod()
        {
            if (_tableComponent != null)
                _tableContainer = _tableComponent.ScrollContainer;
        }

        protected override int GetCurrentItemsCount() => _skills.Count;

        protected override async Task OnLoadMoreItemsAsync()
        {
            await LoadSkillsAsync(append: true);
        }

        private async Task LoadSkillsAsync(bool append = false) => await LoadDataAsync(
            _skills,
            () => SkillService.GetSkillsAsync(
                _currentPage,
                searchText: _searchText,
                confirmed: IsConfirmed?.Value,
                skillTypes: SelectedSkillTypes.Select(s => s.Value)
            ),
            append: append
        );

        private async Task FiltersHasChanged()
        {
            ResetPagination();
            await LoadSkillsAsync();
        }

        private async Task SeacrhSkill(string value)
        {
            _searchText = value;
            await FiltersHasChanged();
        }

        private static Dictionary<MenuAction, object> GetTableActions(Skill skill)
        {
            var actions = new Dictionary<MenuAction, object>();

            if (skill.Confirmed) actions.Add(MenuAction.Edit, skill);
            else actions.Add(MenuAction.Confirm, skill.Id);

            actions.Add(MenuAction.Delete, skill);

            return actions;
        }

        private async Task ResetFilters()
        {
            SelectedSkillTypes.Clear();
            IsConfirmed = null;
            await FiltersHasChanged();
        }

        private void ShowSkillModal(Skill? skill = null) => ModalService.Show<SkillModal>(
            ModalType.Center,
            parameters: skill is not null
                ? new Dictionary<string, object> { [nameof(SkillModal.Skill)] = skill }
                : null
        );

        private async Task OnSkillAction(TableActionContext context)
        {
            if (context.Action is MenuAction.Confirm && context.Item is Guid skillId)
            {
                await SkillService.ConfirmSkillAsync(skillId);
            }
            else if (context.Item is Skill skill)
            {
                if (context.Action is MenuAction.Edit) 
                    ShowSkillModal(skill);
                else if (context.Action is MenuAction.Delete)
                    ModalService.ShowConfirmModal(
                        $"Вы действительно хотите удалить \"{skill.Name}\"?",
                        () => SkillService.DeleteSkillAsync(skill),
                        confirmButtonVariant: ButtonVariant.Danger,
                        confirmButtonText: "Удалить"
                    );
            }
        }

        private void SkillHasCreated(Skill newSkill)
        {
            _skills.Add(newSkill);
            ++_totalCount;
            StateHasChanged();
        }

        private void SkillHasUpdated(Skill updatedSkill)
        {
            var skill = _skills.FirstOrDefault(s => s.Id == updatedSkill.Id);
            if (skill is null) return;

            skill.Name = updatedSkill.Name;
            skill.Confirmed = updatedSkill.Confirmed;
            skill.UpdaterId = updatedSkill.UpdaterId;

            StateHasChanged();
        }

        private void SkillHasDeleted(Skill skill)
        {
            if (_skills.Remove(skill))
            {
                --_totalCount;
                StateHasChanged();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            SkillService.OnSkillHasCreated -= SkillHasCreated;
            SkillService.OnSkillHasUpdated -= SkillHasUpdated;
            SkillService.OnSkillHasDeleted -= SkillHasDeleted;

            await ValueTask.CompletedTask;
        }
    }
}
