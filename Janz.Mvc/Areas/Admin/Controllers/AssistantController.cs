using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Assistant;
using Janz.Mvc.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin köməkçisi: dashboard rəqəmlərindən qısa xülasə verir (api/admin/dashboard) və sualı Gemini-yə ötürür (api/admin/assistant). Bütün işçilər görür
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Staff)]
    public class AssistantController : ApiControllerBase
    {
        public AssistantController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Summary()
        {
            var (dashboard, status) = await GetAsync<DashboardVM>("api/admin/dashboard");
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();

            if (dashboard is null)
            {
                return Ok(new { available = false });
            }

            return Ok(new
            {
                available = true,
                pendingOrders = dashboard.PendingOrders,
                ordersToday = dashboard.OrdersToday,
                revenueThisMonth = dashboard.RevenueThisMonth,
                newCustomers = dashboard.NewCustomersThisMonth,
                lowStock = dashboard.LowStock.Take(5).Select(p => new { p.Name, p.Stock })
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ask(string? question)
        {
            var (result, _, status) = await PostAsync<AssistantAnswerUIVM>("api/admin/assistant", new { question });
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();

            return Ok(new { answer = result?.Answer ?? string.Empty });
        }
    }
}
