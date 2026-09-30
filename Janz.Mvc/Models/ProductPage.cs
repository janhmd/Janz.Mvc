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
}
