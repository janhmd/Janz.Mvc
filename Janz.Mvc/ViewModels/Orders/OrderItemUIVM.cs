namespace Janz.Mvc.ViewModels.Orders
{
    public class OrderItemUIVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }

        // Variant ("Pack · 250 g") və üyütmə: səbətdə və sifarişdə adın altında görünür
        public string? VariantLabel { get; set; }
        public string? Grind { get; set; }
    }
}
