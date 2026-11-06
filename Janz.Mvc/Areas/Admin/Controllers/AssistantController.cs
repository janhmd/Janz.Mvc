using Janz.Mvc.Controllers;
using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Areas.Admin.Controllers
{
    // Admin köməkçisi: dashboard rəqəmlərindən qısa xülasə verir (api/admin/dashboard). Bütün işçilər görür
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
    }
}
