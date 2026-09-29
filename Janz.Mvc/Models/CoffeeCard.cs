namespace Janz.Mvc.Models;

public enum CoffeeSticker
{
    None,
    QGrade,
    Milk
}

public record TasteNote(string Name, string CssClass);

public class CoffeeCard
{
    public string Use { get; init; } = "";
    public string Rating { get; init; } = "";
    public string Reviews { get; init; } = "";
    public string Name { get; init; } = "";
    public string Kind { get; init; } = "";
    public string Image { get; init; } = "";
    public bool CoffeeOfTheWeek { get; init; }
    public string? Tag { get; init; }
    public CoffeeSticker Sticker { get; init; }
    public string TasteLead { get; init; } = "";
    public TasteNote[] Notes { get; init; } = [];
    public string? TasteTail { get; init; }
    public int Density { get; init; }
    public int Acidity { get; init; }
    public string? SmallDiscount { get; init; }
    public string? LargeDiscount { get; init; }
    public string? OldPrice { get; init; }
    public string Price { get; init; } = "";
}
