using Janz.Mvc.ViewModels.Cart;
using Janz.Mvc.ViewModels.Orders;
using Janz.Mvc.ViewModels.Refunds;
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
        public async Task<IActionResult> Create(int addressId, string? couponCode, bool useBalance, string? paymentMethod)
        {
            var (order, errors, status) = await PostAsync<OrderUIVM>("api/orders", new { addressId, couponCode, useBalance, paymentMethod });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            if (order is null)
            {
                TempData["CartError"] = string.Join(" ", errors);
                return RedirectToAction("Index", "Cart");
            }

            // Kartla ödəniş: sifariş pul alınandan sonra yaranır, müştəri Stripe-ın səhifəsinə gedir
            if (!string.IsNullOrEmpty(order.PaymentUrl)) return Redirect(order.PaymentUrl);

            return RedirectToAction(nameof(Detail), new { id = order.Id });
        }

        // Stripe-da "geri" basıldı və ya ödəniş alınmadı: sifariş silinir, məhsullar səbətdə qalır
        [HttpGet]
        public async Task<IActionResult> PaymentCancelled(int id)
        {
            var (result, errors, status) = await PostAsync<PaymentResultUIVM>($"api/orders/{id}/abort-payment", new { });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();

            // Arada ödəniş tamamlanıbsa (sifariş yaranıb) o göstərilir
            if (result is { IsPaid: true })
            {
                TempData["Toast"] = "Thank you! The order is paid.";
                return RedirectToAction(nameof(Detail), new { id });
            }

            TempData["CartError"] = "The payment was not completed, so no order was placed. Your items are still in the cart.";
            return RedirectToAction("Index", "Cart");
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

            // Ödəniş tamamlanmayıbsa gözləyən sifariş görünmür: səbətə qayıdılır
            if (paid is not { IsPaid: true })
            {
                var (_, status2) = await GetAsync<OrderUIVM>($"api/orders/{id}");
                if (status2 == HttpStatusCode.NotFound)
                {
                    TempData["CartError"] = "The payment was not completed, so no order was placed. Your items are still in the cart.";
                    return RedirectToAction("Index", "Cart");
                }
            }

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

        // Çatdırılmış sifariş üçün pulu geri istəmək: Admin və ya Support qəbul edəndə məbləğ balansa qayıdır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refund(int id, string? reason)
        {
            var (refund, errors, status) = await PostAsync<RefundUIVM>($"api/orders/{id}/refund", new { reason });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (status == HttpStatusCode.NotFound) return NotFound();

            if (refund is null) TempData["RefundError"] = string.Join(" ", errors);
            else TempData["Toast"] = "Your refund request was sent. We will answer soon.";

            return RedirectToAction(nameof(Detail), new { id });
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
