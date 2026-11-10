using Janz.Mvc.ViewModels.Pages;
using Janz.Mvc.ViewModels.Settings;
using Janz.Mvc.ViewModels.Tiles;
using Microsoft.Extensions.Caching.Memory;

namespace Janz.Mvc.Helpers
{
    // Menyu, footer və əlaqə məlumatı hər səhifədə lazımdır: API-yə hər dəfə getməmək üçün yadda saxlanılır,
    // admin paneldə dəyişəndə Reset ilə dərhal təzələnir
    public class SiteContent
    {
        private const string PagesKey = "site-pages";
        private const string SettingKey = "site-setting";
        private const string TilesKeyPrefix = "site-tiles-";
        private static readonly string[] TilePlacements = { "home", "coffee", "tea", "chocolate", "sets" };

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

        // Home və kataloq səhifələrindəki kartlar (api/tiles?placement=...). API işləmirsə boş siyahı qayıdır, səhifə kartsız açılır
        public async Task<List<SiteTileUIVM>> TilesAsync(string placement)
        {
            var key = TilesKeyPrefix + placement;
            if (_cache.TryGetValue(key, out List<SiteTileUIVM>? tiles) && tiles is not null) return tiles;

            try
            {
                tiles = await _httpClientFactory.CreateClient("JanzApi").GetFromJsonAsync<List<SiteTileUIVM>>($"api/tiles?placement={Uri.EscapeDataString(placement)}");
            }
            catch (HttpRequestException)
            {
            }
            catch (System.Text.Json.JsonException)
            {
            }

            if (tiles is null) return new List<SiteTileUIVM>();
            _cache.Set(key, tiles, TimeSpan.FromMinutes(5));
            return tiles;
        }

        public void Reset()
        {
            _cache.Remove(PagesKey);
            _cache.Remove(SettingKey);
            foreach (var placement in TilePlacements)
            {
                _cache.Remove(TilesKeyPrefix + placement);
            }
        }
    }
}
