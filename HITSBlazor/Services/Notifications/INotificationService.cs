using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Utils;

namespace HITSBlazor.Services.Notifications
{
    public interface INotificationService
    {
        // Получение уведомлений
        Task<List<Notification>> GetAllNotificationsAsync();
        Task<List<Notification>> GetUnreadNotificationsAsync();
        Task<List<Notification>> GetReadNotificationsAsync();
        Task<List<Notification>> GetFavoriteNotificationsAsync();
        Task<Notification?> GetNotificationByIdAsync(Guid id);
        Task<int> GetUnreadCountAsync();

        // Действия с уведомлениями
        Task<ServiceResponse<Notification>> MarkAsReadAsync(Guid notificationId);
        Task<ServiceResponse<bool>> MarkAllAsReadAsync();
        Task<ServiceResponse<Notification>> AddToFavoritesAsync(Guid notificationId);
        Task<ServiceResponse<Notification>> RemoveFromFavoritesAsync(Guid notificationId);
        Task<ServiceResponse<Notification>> MarkAsShowedAsync(Guid notificationId);

        // События для обновления UI
        event Action? OnNotificationsUpdated;
        event Action<int>? OnUnreadCountChanged;
    }
}
