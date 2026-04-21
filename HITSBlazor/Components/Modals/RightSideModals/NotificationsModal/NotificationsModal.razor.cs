using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.RightSideModals.NotificationsModal
{
    public partial class NotificationsModal
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private NotificationCategory _activeCategory = NotificationCategory.All;

        private List<Notification> _notifications = [];

        protected override async Task OnInitializedAsync()
        {
            return;
        }

        private void ChangeCategory(NotificationCategory newCategory)
        {
            _activeCategory = newCategory;
        }
    }
}
