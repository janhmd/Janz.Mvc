namespace Janz.Mvc.ViewModels.Products
{
    public class WishlistPageVM
    {
        // Seçilmiş məhsullar: ən son seçilən əvvəldə
        public List<ProductUIVM> Products { get; set; } = new List<ProductUIVM>();

        // "Notes": jurnalda saxlanmış məqalələr
        public List<Janz.Mvc.ViewModels.Journal.ArticleCardUIVM> Notes { get; set; } = new List<Janz.Mvc.ViewModels.Journal.ArticleCardUIVM>();
    }
}
