namespace Janz.Mvc.ViewModels.Products
{
    public class HomeIndexVM
    {
        public List<ProductUIVM> Coffee { get; set; } = new List<ProductUIVM>();
        public List<ProductUIVM> Sets { get; set; } = new List<ProductUIVM>();
    }
}
