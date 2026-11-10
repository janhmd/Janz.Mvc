using Janz.Mvc.ViewModels.Profile;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.ViewComponents
{
    // Admin panelin yuxarısındakı hesab: rolu olan istifadəçinin profildəki şəkli (yoxdursa adın ilk hərfi)
    public class AdminUserViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminUserViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string? avatar = null;
            try
            {
                var profile = await _httpClientFactory.CreateClient("JanzApi").GetFromJsonAsync<ProfileUIVM>("api/profile");
                avatar = profile?.AvatarPath;
            }
            catch (HttpRequestException)
            {
                // API işləmirsə hərf göstərilir, panel yenə də açılır
            }
            catch (System.Text.Json.JsonException)
            {
            }

            return View((object?)avatar);
        }
    }
}
