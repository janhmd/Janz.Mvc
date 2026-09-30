namespace Janz.Mvc.Models;

public class ProductPage
{
    public string? Name { get; set; }
    public string? Kind { get; set; }
    public string? Use { get; set; }
    public string? Rating { get; set; }
    public string? Reviews { get; set; }
    public bool CoffeeOfTheWeek { get; set; }
    public string? Tag { get; set; }
    public string? TasteLead { get; set; }
    public TasteNote[] Notes { get; set; } = [];
    public int Density { get; set; }
    public int Acidity { get; set; }
    public string? OldPrice { get; set; }
    public string? Price { get; set; }
    public string? DeliveryWhen { get; set; }
    public string? Roast { get; set; }
    public string? Region { get; set; }
    public string? Process { get; set; }
    public string? Roaster { get; set; }
    public string? Caffeine { get; set; }
    public string? Summary { get; set; }
    public string? Taste { get; set; }
}
