using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Controllers
{
    // Header-dəki zəng və hesabdakı bölmə: oxundu işarəsi (fetch). Siyahını Header və Profile özləri API-dən alır
    [Authorize]
    public class NotificationsController : ApiControllerBase
    {
        public NotificationsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Read(int id)
        {
            var (success, _, status) = await PostAsync($"api/notifications/{id}/read", new { });
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();

            return success ? Ok(new { }) : NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReadAll()
        {
            var (success, _, status) = await PostAsync("api/notifications/read-all", new { });
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();

            return success ? Ok(new { }) : BadRequest();
        }
    }
}
