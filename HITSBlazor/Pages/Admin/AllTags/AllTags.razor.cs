using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Modals.CenterModals.TagModal;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Tags;
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

        private string _searchText = string.Empty;

        private bool? IsConfirmed { get; set; }

        private List<Tag> _tags = [];

        private readonly List<TableHeaderItem> _tagsTableHeader =
        [
            new TableHeaderItem { Text = "Цвет",        ColumnClass = "col-1", InCentered=true  },
            new TableHeaderItem { Text = "Название",    ColumnClass = "col-5"                   },
            new TableHeaderItem { Text = "Статус",      ColumnClass = "col-5"                   }
        ];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            await LoadTagsAsync();
            TagService.OnTagsStateChanged += LoadTagsAsync;
            TagService.OnTagsStateUpdated += StateHasChanged;

            _isLoading = false;
        }
        private async Task LoadTagsAsync()
        {
            _tags = await TagService.GetTagsAsync(
                searchText: _searchText,
                confirmed: IsConfirmed
            );
            StateHasChanged();
        }

        private async Task SeacrhTag(string value)
        {
            _searchText = value;
            await LoadTagsAsync();
        }

        private static Dictionary<MenuAction, object> GetTableActions(Tag tag)
        {
            var actions = new Dictionary<MenuAction, object>();

            if (tag.Confirmed)
                actions.Add(MenuAction.Edit, tag);
            else
                actions.Add(MenuAction.Confirm, tag.Id);

            actions.Add(MenuAction.Delete, tag);

            return actions;
        }

        private async Task ResetFilters()
        {
            IsConfirmed = null;
            await LoadTagsAsync();
        }

        private void ShowTagModal(Tag? tag = null)
        {
            if (tag is not null)
            {
                ModalService.Show<TagModal>(
                    ModalType.Center,
                    parameters: new Dictionary<string, object> { [nameof(TagModal.Tag)] = tag }
                );
            }
            else
            {
                ModalService.Show<TagModal>(ModalType.Center);
            }
        }

        private async Task OnTagAction(TableActionContext context)
        {
            if (context.Action == MenuAction.Confirm)
            {
                if (context.Item is Guid tagId)
                    await TagService.ConfirmTagAsync(tagId);
            }
            else if (context.Action == MenuAction.Edit)
            {
                if (context.Item is Tag tag)
                    ShowTagModal(tag);
            }
            else if (context.Action == MenuAction.Delete)
            {
                if (context.Item is Tag tag)
                    ModalService.ShowDeleteModal(
                        tag.Name,
                        () => TagService.DeleteTagAsync(tag)
                    );
            }
        }

        public void Dispose()
        {
            TagService.OnTagsStateChanged -= LoadTagsAsync;
            TagService.OnTagsStateUpdated -= StateHasChanged;
        }
    }
}
