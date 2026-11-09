using Janz.Mvc.ViewModels.Products;

namespace Janz.Mvc.Helpers
{
    // "Choose coffee in a minute": 3 cavabdan (içki növü, hazırlanma üsulu, dad) ən uyğun qəhvələri seçir
    public static class CoffeePicker
    {
        public static readonly string[] Drinks = { "black", "milk", "both" };
        public static readonly string[] Tastes = { "classic", "bright", "funky" };

        public static (List<ProductUIVM> Products, bool Exact) Choose(IEnumerable<ProductUIVM> coffee, string? drink, string? method, string? taste)
        {
            var available = coffee.Where(m => m.IsVisible && (m.DefaultVariant?.Stock ?? m.Stock) > 0).ToList();

            var scored = available
                .Select(m => (Product: m, Score: Score(m, drink, method, taste)))
                .OrderByDescending(m => m.Score)
                .ThenByDescending(m => m.Product.Rating ?? 0)
                .ToList();

            var best = scored.Count == 0 ? 0 : scored[0].Score;
            if (best <= 0)
            {
                // Dəqiq uyğunluq yoxdur: ən yüksək reytinqlilər göstərilir
                return (available.OrderByDescending(m => m.Rating ?? 0).Take(3).ToList(), false);
            }

            return (scored.Where(m => m.Score > 0).Take(3).Select(m => m.Product).ToList(), true);
        }

        private static int Score(ProductUIVM product, string? drink, string? method, string? taste)
        {
            var score = 0;

            // Hazırlanma üsulu ən vacib şərtdir
            if (!string.IsNullOrWhiteSpace(method) && product.BrewMethods.Any(m => m.Equals(method, StringComparison.OrdinalIgnoreCase)))
            {
                score += 4;
            }

            var forMilk = Has(product.Use, "milk") || product.Tags.Any(t => Has(t.Name, "milk"));
            var darkRoast = Has(product.Roast, "dark") || Has(product.Roast, "milk");
            if (drink == "milk")
            {
                if (forMilk) score += 3;
                if (darkRoast) score += 1;
            }
            else if (drink == "black")
            {
                if (!forMilk) score += 1;
                if (!darkRoast) score += 1;
            }

            var group = product.TasteGroup ?? string.Empty;
            switch (taste)
            {
                case "classic":
                    if (group == "Chocolate / Nuts") score += 3;
                    if ((product.Density ?? 0) >= 50) score += 1;
                    break;
                case "bright":
                    if (group == "Fruits / Berries" || group == "Flowers / Citrus") score += 3;
                    if ((product.Acidity ?? 0) >= 50) score += 1;
                    break;
                case "funky":
                    if (group == "Alcohol / Funky" || Has(product.Processing, "ferment") || Has(product.Processing, "anaerob")
                        || Has(product.Processing, "funky") || Has(product.Processing, "carbonic"))
                    {
                        score += 3;
                    }
                    break;
            }

            return score;
        }

        private static bool Has(string? text, string word) => text is not null && text.Contains(word, StringComparison.OrdinalIgnoreCase);
    }
}
