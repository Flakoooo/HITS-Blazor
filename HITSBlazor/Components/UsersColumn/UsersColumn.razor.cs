using HITSBlazor.Components.Typography;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Entities;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.UsersColumn
{
    public partial class UsersColumn
    {
        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public string HeaderText { get; set; } = string.Empty;

        [Parameter]
        public List<User> AllUsers { get; set; } = [];

        [Parameter]
        public EventCallback<List<User>> AllUsersChanged { get; set; }

        [Parameter]
        public HashSet<User> AdditionalUsers { get; set; } = [];

        [Parameter]
        public EventCallback<User> SelectedUserChanged { get; set; }

        [Parameter]
        public Func<int, string?, Task<ListDataResponse<User>>>? DataLoaderMethod { get; set; }

        [Parameter]
        public int DebounceDelay { get; set; } = 0;

        [Parameter]
        public Func<User, bool>? DisplayPredicate { get; set; }

        [Parameter]
        public Func<User, TextColor?>? DisplayTextColorPredicate { get; set; }

        private readonly List<User> _lodadedUsers = [];

        private string _searchText = string.Empty;
        private List<User> _users = [];

        protected override async Task OnInitializedAsync()
        {
            if (DataLoaderMethod is not null)
            {
                await LoadUsersAsync();
                MarkAsInitialized();
            }
            else
            {
                _users = AllUsers.ToList();
            }
        }

        protected override void OnParametersSet() => SearchUser(_searchText);

        protected override async Task OnLoadMoreItemsAsync()
            => await LoadUsersAsync(append: true);

        protected override int GetCurrentItemsCount() => _lodadedUsers.Count;

        private void SearchUser(string searchText)
        {
            _searchText = searchText;
            if (string.IsNullOrWhiteSpace(_searchText))
            {
                if (DataLoaderMethod is not null)
                {
                    _users = AdditionalUsers.Concat(_lodadedUsers).ToList();
                }
                else
                {
                    _users = AllUsers.ToList();
                }
            }
            else
            {
                if (DataLoaderMethod is not null)
                {
                    _users = AdditionalUsers.Concat(_lodadedUsers).Where(u => u.FullName.Contains(_searchText, StringComparison.CurrentCultureIgnoreCase)).ToList();
                }
                else
                {
                    _users = AllUsers.Where(u => u.FullName.Contains(_searchText, StringComparison.CurrentCultureIgnoreCase)).ToList();
                }
            }
        }

        private async Task OnElementSelected(User user) => await SelectedUserChanged.InvokeAsync(user);

        private async Task LoadUsersAsync(bool append = false)
        {
            if (DataLoaderMethod is not null)
            {
                await LoadDataAsync(
                    _lodadedUsers,
                    () => DataLoaderMethod.Invoke(_currentPage, _searchText),
                    append: append
                );

                AllUsers = _lodadedUsers.ToList();
                if (AllUsersChanged.HasDelegate)
                    await AllUsersChanged.InvokeAsync(AllUsers);
            }
        }
    }
}
