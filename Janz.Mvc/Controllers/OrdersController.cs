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
        public async Task<IActionResult> Create(int addressId, string? couponCode, bool useBalance)
        {
            var (order, errors, status) = await PostAsync<OrderUIVM>("api/orders", new { addressId, couponCode, useBalance });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (order is null)
            {
                TempData["CartError"] = string.Join(" ", errors);
                return RedirectToAction("Index", "Cart");
            }

            return RedirectToAction(nameof(Detail), new { id = order.Id });
        }

        // Kartla ödəniş: API Stripe sessiyası yaradır, müştəri Stripe-ın səhifəsinə gedir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int id)
        {
            var (session, errors, status) = await PostAsync<PaymentSessionUIVM>($"api/orders/{id}/checkout", new { });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (session is null || string.IsNullOrEmpty(session.Url))
            {
                TempData["PayError"] = errors.Length > 0 ? string.Join(" ", errors) : "The payment could not be started.";
                return RedirectToAction(nameof(Detail), new { id });
            }

            return Redirect(session.Url);
        }

        // Stripe ödənişdən sonra bura qaytarır (session_id ünvanda gəlir), ödəniş API-də Stripe-dan yoxlanılır
        [HttpGet]
        public async Task<IActionResult> Paid(int id, [FromQuery(Name = "session_id")] string? sessionId)
        {
            var (paid, errors, status) = await PostAsync<PaymentResultUIVM>($"api/orders/{id}/confirm-payment", new { sessionId });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (paid is null) TempData["PayError"] = string.Join(" ", errors);
            else TempData["Toast"] = paid.IsPaid ? "Thank you! The order is paid." : "The payment is not finished yet.";

            return RedirectToAction(nameof(Detail), new { id });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(int id, int shopRating, string? shopText, int deliveryRating, string? deliveryText, string? comment)
        {
            var (success, errors, status) = await PutAsync($"api/reviews/order/{id}", new { shopRating, shopText, deliveryRating, deliveryText, comment });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (success) TempData["Toast"] = "Thank you! Your review was saved.";
            else TempData["ReviewError"] = string.Join(" ", errors);

            // Rəy Reviews səhifəsindən göndərilir
            return Redirect("/Reviews#feedback");
        }
    }
}
