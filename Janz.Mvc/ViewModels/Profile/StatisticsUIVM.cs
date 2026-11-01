using Janz.Mvc.ViewModels.Cart;
using Janz.Mvc.ViewModels.Products;

namespace Janz.Mvc.ViewModels.Profile
{
    // Kabinetdəki statistika: ləğv olunmamış sifarişlərdən hesablanır
    public class StatisticsUIVM
    {
        public int OrderCount { get; set; }
        public int ItemCount { get; set; }
        public decimal Spent { get; set; }
        public decimal AverageOrder { get; set; }
        public decimal Saved { get; set; }
        public DiscountLevelUIVM? Level { get; set; }

        // Ən çox alınan məhsullar (say üzrə)
        public List<ProductUIVM> TopProducts { get; set; } = new List<ProductUIVM>();
    }
}
