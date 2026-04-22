namespace HITSBlazor.Components.GlobalNotification
{
    public class GlobalNotificationItem : IDisposable
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; } = NotificationType.Info;
        public string AlertAnimation { get; set; } = "alert-slide-in";
        public int Index { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public CancellationTokenSource? AutoRemoveCts { get; set; }

        public void Dispose()
        {
            AutoRemoveCts?.Cancel();
            AutoRemoveCts?.Dispose();
        }
    }
}
