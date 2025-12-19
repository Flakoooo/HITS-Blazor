using Microsoft.JSInterop;

namespace HITSBlazor.Utils
{
    public class CookieService(IJSRuntime jsRuntime) : ICookieService
    {
        private readonly IJSRuntime _jsRuntime = jsRuntime;

        public async Task<string?> GetCookieAsync(string name)
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string>("getCookie", name);
            }
            catch (JSException)
            {
                return null;
            }
        }

        public async Task<bool> HasCookieAsync(string name)
        {
            var cookie = await GetCookieAsync(name);
            return !string.IsNullOrEmpty(cookie);
        }

        public async Task<bool> DeleteCookie(string name)
        {
            await _jsRuntime.InvokeVoidAsync("deleteCookie", name);

            return await HasCookieAsync(name);
        }
    }
}
