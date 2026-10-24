namespace Janz.Mvc.ViewModels.Categories
{
    public class CategoryUIVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
}
