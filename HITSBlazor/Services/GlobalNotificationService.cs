namespace HITSBlazor.Services
{
    public class GlobalNotificationService
    {
        public event Action<string>? OnErrorNotification;
        public event Action<string>? OnSuccessNotification;
        public event Action? OnClearNotification;

        public void ShowError(string message)
        {
            OnErrorNotification?.Invoke(message);
        }

        public void ShowSuccess(string message)
        {
            OnSuccessNotification?.Invoke(message);
        }

        public void Clear() => OnClearNotification?.Invoke();
    }
}
