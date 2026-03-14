using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.CenterModals.UsersGroupModal
{
    public partial class UsersGroupModal
    {
        [Parameter]
        public Guid? UsersGroupId { get; set; } = null!;

        private bool _isLoading = true;
        private bool _submitting = false;

        private string UsersGroupName { get; set; } = string.Empty;
    }
}
