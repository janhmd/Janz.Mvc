using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Community;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: Community postları, şərhlər və şikayətlər (api/admin/community)
    [Area("Admin")]
    [Authorize(Roles = AppRoles.ShopManagers)]
    public class CommunityController : ApiControllerBase
    {
        public CommunityController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? tab)
        {
            var model = new AdminCommunityVM { Tab = tab == "comments" || tab == "reports" ? tab : "posts" };

            HttpStatusCode? status;
            if (model.Tab == "comments")
            {
                List<AdminCommunityCommentVM>? comments;
                (comments, status) = await GetAsync<List<AdminCommunityCommentVM>>("api/admin/community/comments");
                if (comments is not null) model.Comments = comments;
                else ViewBag.Error = UnavailableMessage;
            }
            else if (model.Tab == "reports")
            {
                List<AdminCommunityReportVM>? reports;
                (reports, status) = await GetAsync<List<AdminCommunityReportVM>>("api/admin/community/reports");
                if (reports is not null) model.Reports = reports;
                else ViewBag.Error = UnavailableMessage;
            }
            else
            {
                List<AdminCommunityPostVM>? posts;
                (posts, status) = await GetAsync<List<AdminCommunityPostVM>>("api/admin/community/posts");
                if (posts is not null) model.Posts = posts;
                else ViewBag.Error = UnavailableMessage;
            }

            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> HidePost(int id, string? returnUrl) => SendAsync(() => PutAsync($"api/admin/community/posts/{id}/hidden", new { isHidden = true }), "The post was hidden from the site.", returnUrl);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> ShowPost(int id, string? returnUrl) => SendAsync(() => PutAsync($"api/admin/community/posts/{id}/hidden", new { isHidden = false }), "The post is visible on the site again.", returnUrl);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> DeletePost(int id, string? returnUrl) => SendAsync(() => DeleteAsync($"api/admin/community/posts/{id}"), "The post was deleted.", returnUrl);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> HideComment(int id, string? returnUrl) => SendAsync(() => PutAsync($"api/admin/community/comments/{id}/hidden", new { isHidden = true }), "The comment was hidden from the site.", returnUrl);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> ShowComment(int id, string? returnUrl) => SendAsync(() => PutAsync($"api/admin/community/comments/{id}/hidden", new { isHidden = false }), "The comment is visible on the site again.", returnUrl);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteComment(int id, string? returnUrl) => SendAsync(() => DeleteAsync($"api/admin/community/comments/{id}"), "The comment was deleted.", returnUrl);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Resolve(int id, string? returnUrl) => SendAsync(() => PutAsync($"api/admin/community/reports/{id}/resolve", new { }), "The report was marked as resolved.", returnUrl);

        private async Task<IActionResult> SendAsync(Func<Task<(bool Success, string[] Errors, HttpStatusCode? Status)>> call, string message, string? returnUrl)
        {
            var (success, errors, status) = await call();
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (success) TempData["AdminSuccess"] = message;
            else TempData["AdminError"] = status == HttpStatusCode.NotFound ? "The item was not found." : string.Join(" ", errors);

            return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToAction(nameof(Index));
        }
    }
}
