namespace HITSBlazor.Utils
{
    public interface ICookieService
    {
        Task<string?> GetCookieAsync(string name);
        Task<bool> HasCookieAsync(string name);
        Task<bool> DeleteCookie(string name);
    }
}
