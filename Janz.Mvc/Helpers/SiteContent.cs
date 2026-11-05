using Janz.Mvc.ViewModels.Pages;
using Janz.Mvc.ViewModels.Settings;
using Microsoft.Extensions.Caching.Memory;

namespace Janz.Mvc.Helpers
{
    // Menyu, footer və əlaqə məlumatı hər səhifədə lazımdır: API-yə hər dəfə getməmək üçün yadda saxlanılır,
    // admin paneldə dəyişəndə Reset ilə dərhal təzələnir
    public class SiteContent
    {
        private const string PagesKey = "site-pages";
        private const string SettingKey = "site-setting";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public SiteContent(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }

        public async Task<List<SitePageLinkUIVM>> PagesAsync()
        {
            if (_cache.TryGetValue(PagesKey, out List<SitePageLinkUIVM>? pages) && pages is not null) return pages;

            try
            {
                pages = await _httpClientFactory.CreateClient("JanzApi").GetFromJsonAsync<List<SitePageLinkUIVM>>("api/pages");
            }
            catch (HttpRequestException)
            {
            }
            catch (System.Text.Json.JsonException)
            {
            }

            if (pages is null) return new List<SitePageLinkUIVM>();
            _cache.Set(PagesKey, pages, TimeSpan.FromMinutes(5));
            return pages;
        }

        public async Task<SiteSettingUIVM> SettingAsync()
        {
            if (_cache.TryGetValue(SettingKey, out SiteSettingUIVM? setting) && setting is not null) return setting;

            try
            {
                setting = await _httpClientFactory.CreateClient("JanzApi").GetFromJsonAsync<SiteSettingUIVM>("api/settings");
            }
            catch (HttpRequestException)
            {
            }
            catch (System.Text.Json.JsonException)
            {
            }

            if (setting is null) return new SiteSettingUIVM();
            _cache.Set(SettingKey, setting, TimeSpan.FromMinutes(5));
            return setting;
        }

        public void Reset()
        {
            _cache.Remove(PagesKey);
            _cache.Remove(SettingKey);
        }
    }
}
