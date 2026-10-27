namespace Janz.Mvc.ViewModels.Products
{
    // Admin: məhsulu dəyişmək. Sahələr yeni məhsulla eynidir, üstəlik köhnə şəkillər
    public class ProductEditVM : ProductCreateVM
    {
        public int Id { get; set; }

        // Qalereyadan silinəcək köhnə şəkillər
        public List<string> RemoveImages { get; set; } = new List<string>();

        // Səhifədə göstərmək üçün (formadan gəlmir, API-dən doldurulur)
        public string ImagePath { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new List<string>();
    }
}
