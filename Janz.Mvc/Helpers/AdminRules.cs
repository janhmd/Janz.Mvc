namespace Janz.Mvc.Helpers
{
    // Admin paneldə formaların limitləri. API-dəki ImageFileRules və ProductRules ilə eyni olmalıdır:
    // burada yoxlamaq faylı API-yə göndərmədən səhvi göstərməyə imkan verir
    public static class AdminRules
    {
        public const int MaxImageBytes = 2 * 1024 * 1024; // 2 MB
        public const int MaxGallery = 6;
        public const int MaxVariants = 8;
        public const int MaxVariantImages = 6;
        public const int LowStock = 5;                     // bundan az olan stok "az qalıb" sayılır (dashboard ilə eyni)

        public static readonly string[] ImageTypes = { "image/png", "image/jpeg", "image/webp" };
        public const string ImageAccept = "image/png,image/jpeg,image/webp";
    }
}
