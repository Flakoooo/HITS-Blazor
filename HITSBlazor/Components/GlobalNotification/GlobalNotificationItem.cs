namespace HITSBlazor.Components.GlobalNotification
{
    public class GlobalNotificationItem
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = "";
        public bool IsError { get; set; }
        public string AlertAnimation { get; set; } = "alert-slide-in";
        public int Index { get; set; }

        public string AlertClass => IsError ? "alert-danger" : "alert-success";
    }
}
