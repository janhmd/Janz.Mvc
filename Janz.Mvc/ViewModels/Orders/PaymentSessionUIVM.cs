namespace Janz.Mvc.ViewModels.Orders
{
    // api/orders/{id}/checkout cavabı: Stripe-ın ödəniş səhifəsi
    public class PaymentSessionUIVM
    {
        public string Url { get; set; } = string.Empty;
    }
}
