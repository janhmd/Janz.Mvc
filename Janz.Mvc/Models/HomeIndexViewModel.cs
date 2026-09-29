namespace Janz.Mvc.Models;

public class HomeIndexViewModel
{
    public IReadOnlyList<CoffeeCard> NewCoffees { get; init; } = [];
    public IReadOnlyList<CoffeeCard> PopularCoffees { get; init; } = [];
}
