using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Services.Notifications
{
    public class MockNotificationService : INotificationService
    {
        private readonly IAuthService _authService;
        private List<Notification> _notifications = [];

        public event Action? OnNotificationsUpdated;
        public event Action<int>? OnUnreadCountChanged;

        public MockNotificationService(IAuthService authService)
        {
            _authService = authService;
            _authService.OnAuthStateChanged += OnAuthStateChanged;
        }

        private async void OnAuthStateChanged()
        {
            if (_authService.IsAuthenticated)
                await EnsureNotificationsLoaded();
            else
                _notifications.Clear();

            OnNotificationsUpdated?.Invoke();
        }

        private async Task EnsureNotificationsLoaded()
        {
            if (_notifications.Count == 0 && _authService.CurrentUser != null)
                _notifications = [.. MockNotifications.GetAllNotificationsByUserId(_authService.CurrentUser.Id)];
        }

        public async Task<List<Notification>> GetAllNotificationsAsync()
        {
            await EnsureNotificationsLoaded();
            return [.. _notifications.OrderByDescending(n => n.CreatedAt)];
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync()
            => [.._notifications.Where(n => !n.IsReaded)];

        public async Task<List<Notification>> GetReadNotificationsAsync()
            => [.. _notifications.Where(n => n.IsReaded)];

        public async Task<List<Notification>> GetFavoriteNotificationsAsync()
            => [.. _notifications.Where(n => n.IsFavorite)];

        public async Task<Notification?> GetNotificationByIdAsync(Guid id)
            => _notifications.FirstOrDefault(n => n.Id == id);

        public async Task<int> GetUnreadCountAsync()
            => (await GetUnreadNotificationsAsync()).Count;

        public async Task<ServiceResponse<Notification>> MarkAsReadAsync(Guid notificationId)
        {
            await EnsureNotificationsLoaded();
            var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);

            if (notification == null)
                return ServiceResponse<Notification>.Failure("Уведомление не найдено");

            if (!notification.IsReaded)
            {
                notification.IsReaded = true;

                OnUnreadCountChanged?.Invoke(await GetUnreadCountAsync());
                OnNotificationsUpdated?.Invoke();
            }

            return ServiceResponse<Notification>.Success(notification);
        }

        public async Task<ServiceResponse<bool>> MarkAllAsReadAsync()
        {
            await EnsureNotificationsLoaded();
            var currentUser = _authService.CurrentUser;

            if (currentUser == null)
                return ServiceResponse<bool>.Failure("Пользователь не авторизован");

            var userNotifications = _notifications
                .Where(n => n.UserId == currentUser.Id && !n.IsReaded)
                .ToList();

            foreach (var notification in userNotifications)
                notification.IsReaded = true;

            OnUnreadCountChanged?.Invoke(0);
            OnNotificationsUpdated?.Invoke();

            return ServiceResponse<bool>.Success(true, $"Отмечено как прочитанные: {userNotifications.Count} уведомлений");
        }

        public async Task<ServiceResponse<Notification>> AddToFavoritesAsync(Guid notificationId)
        {
            await EnsureNotificationsLoaded();
            var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);

            if (notification == null)
                return ServiceResponse<Notification>.Failure("Уведомление не найдено");

            if (!notification.IsFavorite)
            {
                notification.IsFavorite = true;
                OnNotificationsUpdated?.Invoke();
            }

            return ServiceResponse<Notification>.Success(notification);
        }

        public async Task<ServiceResponse<Notification>> RemoveFromFavoritesAsync(Guid notificationId)
        {
            await EnsureNotificationsLoaded();
            var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);

            if (notification == null)
                return ServiceResponse<Notification>.Failure("Уведомление не найдено");

            if (notification.IsFavorite)
            {
                notification.IsFavorite = false;
                OnNotificationsUpdated?.Invoke();
            }

            return ServiceResponse<Notification>.Success(notification);
        }

        public async Task<ServiceResponse<Notification>> MarkAsShowedAsync(Guid notificationId)
        {
            await EnsureNotificationsLoaded();
            var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);

            if (notification == null)
                return ServiceResponse<Notification>.Failure("Уведомление не найдено");

            if (!notification.IsShowed)
            {
                notification.IsShowed = true;
                OnNotificationsUpdated?.Invoke();
            }

            return ServiceResponse<Notification>.Success(notification);
        }

        public void Dispose()
            => _authService.OnAuthStateChanged -= OnAuthStateChanged;
    }
}
