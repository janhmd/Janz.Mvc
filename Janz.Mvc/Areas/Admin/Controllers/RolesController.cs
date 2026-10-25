using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: rollar və hər roldakı istifadəçi sayı (yalnız SuperAdmin)
    [Area("Admin")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class RolesController : ApiControllerBase
    {
        public RolesController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (roles, code) = await GetAsync<List<RoleUIVM>>("api/admin/roles");
            if (code == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (roles is null) ViewBag.Error = UnavailableMessage;

            return View(roles ?? new List<RoleUIVM>());
        }
    }
}
