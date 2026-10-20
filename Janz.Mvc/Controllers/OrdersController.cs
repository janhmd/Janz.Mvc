using Janz.Mvc.ViewModels.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Controllers
{
    // Sifarişlər: səbətdən sifariş vermək və sifarişin səhifəsi. Məlumat API-dən gəlir: api/orders.
    // Sifariş tarixçəsi Profile səhifəsində göstərilir
    [Authorize]
    public class OrdersController : ApiControllerBase
    {
        public OrdersController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        // Səbətdəki hər şey seçilən ünvana sifariş olunur; alınmasa (stok, boş səbət) xəta səbətdə göstərilir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int addressId)
        {
            var (order, errors, status) = await PostAsync<OrderUIVM>("api/orders", new { addressId });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (order is null)
            {
                TempData["CartError"] = string.Join(" ", errors);
                return RedirectToAction("Index", "Cart");
            }

            return RedirectToAction(nameof(Detail), new { id = order.Id });
        }

        // Başqasının sifarişi API-də 404 qaytarır: xəta səhifəsi göstərilir
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var (order, status) = await GetAsync<OrderUIVM>($"api/orders/{id}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (order is null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
