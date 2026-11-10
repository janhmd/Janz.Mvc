using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Products;
using Janz.Mvc.ViewModels.Tags;
using Xunit;

namespace Janz.Mvc.Tests
{
    public class CoffeePickerTests
    {
        private static ProductUIVM Coffee(int id, string name, int stock = 10, decimal? rating = null) =>
            new() { Id = id, Name = name, Stock = stock, Rating = rating, IsVisible = true };

        [Fact]
        public void Choose_PrefersTheBrewMethodOverEverythingElse()
        {
            var espresso = Coffee(1, "Espresso blend");
            espresso.BrewMethods.Add("Espresso");
            var filter = Coffee(2, "Filter roast");
            filter.BrewMethods.Add("Filter");
            filter.TasteGroup = "Chocolate / Nuts";

            var (products, exact) = CoffeePicker.Choose(new[] { filter, espresso }, "both", "espresso", "classic");

            Assert.True(exact);
            Assert.Equal(1, products[0].Id);
        }

        [Fact]
        public void Choose_MatchesMilkDrinksByUseOrTag()
        {
            var forMilk = Coffee(1, "Milk coffee");
            forMilk.Use = "For milk drinks";
            var tagged = Coffee(2, "Tagged coffee");
            tagged.Tags.Add(new TagUIVM { Name = "For milk drinks", Slug = "milk-drinks" });
            var plain = Coffee(3, "Plain coffee");

            var (products, exact) = CoffeePicker.Choose(new[] { plain, tagged, forMilk }, "milk", null, null);

            Assert.True(exact);
            Assert.Equal(new[] { 1, 2 }, products.Select(m => m.Id).OrderBy(m => m).ToArray());
        }

        [Theory]
        [InlineData("classic", "Chocolate / Nuts")]
        [InlineData("bright", "Fruits / Berries")]
        [InlineData("bright", "Flowers / Citrus")]
        [InlineData("funky", "Alcohol / Funky")]
        public void Choose_MatchesTheTasteGroup(string taste, string group)
        {
            var match = Coffee(1, "Match");
            match.TasteGroup = group;
            var other = Coffee(2, "Other");
            other.TasteGroup = "Spices";

            var (products, exact) = CoffeePicker.Choose(new[] { other, match }, null, null, taste);

            Assert.True(exact);
            Assert.Equal(1, Assert.Single(products).Id);
        }

        [Fact]
        public void Choose_TreatsFermentedProcessingAsFunky()
        {
            var fermented = Coffee(1, "Natty");
            fermented.Processing = "Anaerobic natural";

            var (products, exact) = CoffeePicker.Choose(new[] { fermented, Coffee(2, "Washed") }, null, null, "funky");

            Assert.True(exact);
            Assert.Equal(1, Assert.Single(products).Id);
        }

        [Fact]
        public void Choose_SkipsHiddenAndSoldOutCoffee()
        {
            var hidden = Coffee(1, "Hidden");
            hidden.IsVisible = false;
            hidden.BrewMethods.Add("Espresso");
            var soldOut = Coffee(2, "Sold out", stock: 0);
            soldOut.BrewMethods.Add("Espresso");
            var available = Coffee(3, "Available");
            available.BrewMethods.Add("Espresso");

            var (products, _) = CoffeePicker.Choose(new[] { hidden, soldOut, available }, null, "espresso", null);

            Assert.Equal(3, Assert.Single(products).Id);
        }

        [Fact]
        public void Choose_UsesTheVariantStock_WhenTheCoffeeHasVariants()
        {
            var coffee = Coffee(1, "Variants", stock: 0);
            coffee.BrewMethods.Add("Filter");
            coffee.Variants.Add(new ProductVariantUIVM { Id = 1, Stock = 4 });

            var (products, _) = CoffeePicker.Choose(new[] { coffee }, null, "filter", null);

            Assert.Single(products);
        }

        [Fact]
        public void Choose_FallsBackToTopRatedCoffee_WhenNothingMatches()
        {
            var low = Coffee(1, "Low", rating: 3.5m);
            var high = Coffee(2, "High", rating: 4.9m);

            var (products, exact) = CoffeePicker.Choose(new[] { low, high }, "milk", "aeropress", "funky");

            Assert.False(exact);
            Assert.Equal(2, products[0].Id);
        }

        [Fact]
        public void Choose_ReturnsAtMostThreeCoffees()
        {
            var many = Enumerable.Range(1, 6).Select(i =>
            {
                var coffee = Coffee(i, "Coffee " + i);
                coffee.BrewMethods.Add("Espresso");
                return coffee;
            });

            var (products, _) = CoffeePicker.Choose(many, null, "espresso", null);

            Assert.Equal(3, products.Count);
        }

        [Fact]
        public void Choose_ReturnsNothing_WhenThereIsNoCoffee()
        {
            var (products, exact) = CoffeePicker.Choose(Array.Empty<ProductUIVM>(), "black", "filter", "classic");

            Assert.Empty(products);
            Assert.False(exact);
        }
    }
}
