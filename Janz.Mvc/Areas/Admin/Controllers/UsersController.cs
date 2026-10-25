using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: istifadəçilər (api/admin/users). Bloklamaq Admin və SuperAdmin-də, rolu dəyişmək yalnız SuperAdmin-də
    [Area("Admin")]
    [Authorize(Roles = AppRoles.OrderManagers)]
    public class UsersController : ApiControllerBase
    {
        public static readonly string[] Roles = { AppRoles.User, AppRoles.Support, AppRoles.Editor, AppRoles.Admin, AppRoles.SuperAdmin };

        public UsersController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? role, bool? blocked, string? search)
        {
            if (role is not null && !Roles.Contains(role)) role = null;
            search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

            var url = "api/admin/users?role=" + Uri.EscapeDataString(role ?? string.Empty)
                + "&search=" + Uri.EscapeDataString(search ?? string.Empty)
                + (blocked is null ? string.Empty : "&blocked=" + (blocked.Value ? "true" : "false"));
            var (users, code) = await GetAsync<List<UserAdminUIVM>>(url);
            if (code == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (users is null) ViewBag.Error = UnavailableMessage;

            return View(new UserListVM
            {
                Users = (users ?? new List<UserAdminUIVM>()).OrderByDescending(m => m.CreatedAt).ToList(),
                Role = role,
                Blocked = blocked,
                Search = search
            });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var (user, code) = await GetAsync<UserAdminUIVM>($"api/admin/users/{Uri.EscapeDataString(id)}");
            if (code == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (user is null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.ShopManagers)]
        public Task<IActionResult> Block(string id) => SendAsync(id, "block", null, "The user was blocked.");

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.ShopManagers)]
        public Task<IActionResult> Unblock(string id) => SendAsync(id, "unblock", null, "The user was unblocked.");

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppRoles.SuperAdmin)]
        public Task<IActionResult> Role(string id, string? role)
        {
            if (role is null || !Roles.Contains(role))
            {
                TempData["AdminError"] = "Choose a role from the list.";
                return Task.FromResult<IActionResult>(RedirectToAction(nameof(Detail), new { id }));
            }

            return SendAsync(id, "role", new { role }, $"The role is now {role}.");
        }

        // API-nin mesajı (məs. "You can't block yourself.") olduğu kimi göstərilir
        private async Task<IActionResult> SendAsync(string id, string action, object? body, string success)
        {
            var (ok, errors, code) = await PutAsync($"api/admin/users/{Uri.EscapeDataString(id)}/{action}", body ?? new { });
            if (code == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (code == HttpStatusCode.NotFound) return NotFound();

            if (ok) TempData["AdminSuccess"] = success;
            else TempData["AdminError"] = string.Join(" ", errors);

            return RedirectToAction(nameof(Detail), new { id });
        }
    }
}
