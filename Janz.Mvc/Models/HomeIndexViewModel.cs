namespace Janz.Mvc.Models;

public class HomeIndexViewModel
{
    public IReadOnlyList<CoffeeCard> NewCoffees { get; set; } = [];
    public IReadOnlyList<CoffeeCard> PopularCoffees { get; set; } = [];
    public IReadOnlyList<SetCard> Sets { get; set; } = [];
    public string? ReviewCount { get; set; }
    public string? ShopRating { get; set; }
    public string? DeliveryRating { get; set; }
    public IReadOnlyList<ReviewCard> Reviews { get; set; } = [];
}
