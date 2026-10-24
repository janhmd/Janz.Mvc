namespace Janz.Mvc.Helpers
{
    // API-dəki Domain/Constants/ProductBadges.cs ilə eyni olmalıdır (MVC Domain-i görmür)
    public static class ProductBadges
    {
        public const string CoffeeOfTheWeek = "Coffee of the week";
        public const string New = "New";
        public const string Sale = "Sale";
        public const string Limited = "Limited";

        public static readonly string[] All = { CoffeeOfTheWeek, New, Sale, Limited };
    }
}
