namespace HITSBlazor.Utils
{
    public class CacheEntry<T>(T data)
    {
        public T Data { get; set; } = data;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsExpired(TimeSpan lifetime) => DateTime.UtcNow - CreatedAt > lifetime;
    }
}
