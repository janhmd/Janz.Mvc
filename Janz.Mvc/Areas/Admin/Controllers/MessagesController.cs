using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Contact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin panel: müştəri mesajları (api/admin/messages). Support da görür və cavab yazır, cavab müştərinin e-poçtuna gedir
    [Area("Admin")]
    [Authorize(Roles = AppRoles.OrderManagers)]
    public class MessagesController : ApiControllerBase
    {
        public MessagesController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? status)
        {
            if (status is not null && status != "new" && status != "answered") status = null;

            var (messages, code) = await GetAsync<List<ContactMessageVM>>("api/admin/messages?status=" + Uri.EscapeDataString(status ?? string.Empty));
            if (code == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (messages is null) ViewBag.Error = UnavailableMessage;

            ViewBag.Status = status;
            return View(messages ?? new List<ContactMessageVM>());
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var (message, code) = await GetAsync<ContactMessageVM>($"api/admin/messages/{id}");
            if (code == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (message is null) return NotFound();

            return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, ReplyFormVM form)
        {
            if (!ModelState.IsValid)
            {
                TempData["AdminError"] = string.Join(" ", ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction(nameof(Detail), new { id });
            }

            var (success, errors, code) = await PostAsync($"api/admin/messages/{id}/reply", new { reply = form.Reply });
            if (code == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (code == HttpStatusCode.NotFound) return NotFound();

            if (success) TempData["AdminSuccess"] = "The reply was sent by email.";
            else TempData["AdminError"] = string.Join(" ", errors);

            return RedirectToAction(nameof(Detail), new { id });
        }
    }
}
