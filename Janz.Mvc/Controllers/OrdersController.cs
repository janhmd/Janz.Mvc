using Janz.Mvc.ViewModels.Orders;
using Janz.Mvc.ViewModels.Reviews;
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
        public async Task<IActionResult> Create(int addressId, string? couponCode)
        {
            var (order, errors, status) = await PostAsync<OrderUIVM>("api/orders", new { addressId, couponCode });
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

            // Çatdırılmış sifarişdə mağaza və çatdırılma üçün rəy forması göstərilir
            if (order.Status == "Delivered")
            {
                var (review, _) = await GetAsync<OrderReviewUIVM>($"api/reviews/order/{id}");
                ViewBag.Review = review;
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(int id, int shopRating, string? shopText, int deliveryRating, string? deliveryText, string? comment)
        {
            var (success, errors, status) = await PutAsync($"api/reviews/order/{id}", new { shopRating, shopText, deliveryRating, deliveryText, comment });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (success) TempData["Toast"] = "Thank you! Your review was saved.";
            else TempData["ReviewError"] = string.Join(" ", errors);

            return Redirect($"/Orders/Detail/{id}");
        }
    }
}
