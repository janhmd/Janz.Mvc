namespace Janz.Mvc.ViewModels.Products
{
    public class WishlistPageVM
    {
        // Seçilmiş məhsullar: ən son seçilən əvvəldə
        public List<ProductUIVM> Products { get; set; } = new List<ProductUIVM>();
    }
}
