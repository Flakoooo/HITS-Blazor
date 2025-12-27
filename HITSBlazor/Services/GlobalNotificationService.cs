namespace HITSBlazor.Services
{
    public class GlobalNotificationService
    {
        public event Action<string>? OnErrorNotification;
        public event Action<string>? OnSuccessNotification;
        public event Action? OnClearNotification;

        public void ShowError(string message, int duration = 5000)
        {
            OnErrorNotification?.Invoke(message);

            if (duration > 0)
            {
                var timer = new System.Timers.Timer(duration);
                timer.Elapsed += (sender, args) =>
                {
                    OnClearNotification?.Invoke();
                    timer.Dispose();
                };
                timer.AutoReset = false;
                timer.Start();
            }
        }

        public void ShowSuccess(string message, int duration = 5000)
        {
            OnSuccessNotification?.Invoke(message);

            if (duration > 0)
            {
                var timer = new System.Timers.Timer(duration);
                timer.Elapsed += (sender, args) =>
                {
                    OnClearNotification?.Invoke();
                    timer.Dispose();
                };
                timer.AutoReset = false;
                timer.Start();
            }
        }

        public void Clear() => OnClearNotification?.Invoke();
    }
}
