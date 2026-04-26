using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.CenterModals.TagModal;
using HITSBlazor.Components.Tables.TableComponent;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Tags;
using HITSBlazor.Utils.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Admin.AllTags
{
    [Authorize]
    [Route("admin/tags")]
    public partial class AllTags
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private ITagService TagService { get; set; } = null!;

        private bool _isLoading = true;

        private TableComponent? _tableComponent;

        private string _searchText = string.Empty;

        private readonly List<ValueViewModel<bool?>> _confirmedFilterValues =
        [
            new(true, "Утвержден"),
            new(false, "На рассмотрении")
        ];

        private ValueViewModel<bool?>? IsConfirmed { get; set; }

        private readonly List<Tag> _tags = [];

        private readonly List<TableHeaderItem> _tagsTableHeader =
        [
            new TableHeaderItem { Text = "Цвет",        ColumnClass = "col-1", InCentered=true  },
            new TableHeaderItem { Text = "Название",    ColumnClass = "col-5"                   },
            new TableHeaderItem { Text = "Статус",      ColumnClass = "col-5"                   }
        ];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            TagService.OnTagHasCreated += TagHasCreated;
            TagService.OnTagHasUpdated += TagHasUpdated;
            TagService.OnTagHasDeleted += TagHasDeleted;

            await LoadTagsAsync();

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async Task AdditionalAfterRenderMethod()
        {
            if (_tableComponent != null)
                _tableContainer = _tableComponent.ScrollContainer;
        }

        protected override int GetCurrentItemsCount() => _tags.Count;

        protected override async Task OnLoadMoreItemsAsync()
        {
            await LoadTagsAsync(append: true);
        }

        private async Task LoadTagsAsync(bool append = false) => await LoadDataAsync(
            _tags,
            () => TagService.GetTagsAsync(
                _currentPage,
                searchText: _searchText,
                confirmed: IsConfirmed?.Value
            ),
            append: append
        );

        private async Task SeacrhTag(string value)
        {
            _searchText = value;
            await FiltersHasChanged();
        }

        private static Dictionary<MenuAction, object> GetTableActions(Tag tag)
        {
            var actions = new Dictionary<MenuAction, object>();

            if (tag.Confirmed) actions.Add(MenuAction.Edit, tag);
            else actions.Add(MenuAction.Confirm, tag.Id);

            actions.Add(MenuAction.Delete, tag);

            return actions;
        }

        private async Task FiltersHasChanged()
        {
            ResetPagination();
            await LoadTagsAsync();
        }

        private async Task ResetFilters()
        {
            IsConfirmed = null;
            await FiltersHasChanged();
        }

        private void ShowTagModal(Tag? tag = null) => ModalService.Show<TagModal>(
            ModalType.Center,
            parameters: tag is not null
                ? new Dictionary<string, object> { [nameof(TagModal.Tag)] = tag }
                : null
        );

        private async Task OnTagAction(TableActionContext context)
        {
            if (context.Action == MenuAction.Confirm && context.Item is Guid tagId)
            {
                await TagService.ConfirmTagAsync(tagId);
            }
            else if (context.Item is Tag tag)
            {
                if (context.Action == MenuAction.Edit)
                    ShowTagModal(tag);
                else if (context.Action == MenuAction.Delete)
                    ModalService.ShowConfirmModal(
                        $"Вы действительно хотите удалить \"{tag.Name}\"?",
                        () => TagService.DeleteTagAsync(tag),
                        confirmButtonVariant: ButtonVariant.Danger,
                        confirmButtonText: "Удалить"
                    );

            }
        }

        private void TagHasCreated(Tag createdTag)
        {
            _tags.Add(createdTag);
            ++_totalCount;
            StateHasChanged();
        }

        private void TagHasUpdated(Tag updatedTag)
        {
            var tag = _tags.FirstOrDefault(t => t.Id == updatedTag.Id);
            if (tag is null) return;

            tag.Color = updatedTag.Color;
            tag.Confirmed = updatedTag.Confirmed;
            tag.UpdaterId = updatedTag.UpdaterId;

            StateHasChanged();
        }

        private void TagHasDeleted(Tag tag)
        {
            if (_tags.Remove(tag))
            {
                --_totalCount;
                StateHasChanged();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            TagService.OnTagHasCreated -= TagHasCreated;
            TagService.OnTagHasUpdated -= TagHasUpdated;
            TagService.OnTagHasDeleted -= TagHasDeleted;

            await ValueTask.CompletedTask;
        }
    }
}
