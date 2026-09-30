namespace Janz.Mvc.Models;

public enum CoffeeSticker
{
    None,
    QGrade,
    Milk
}

public class TasteNote
{
    public TasteNote(string name, string cssClass)
    {
        Name = name;
        CssClass = cssClass;
    }

    public string Name { get; set; }
    public string CssClass { get; set; }
}

public class CoffeeCard
{
    public string? Use { get; set; }
    public string? Rating { get; set; }
    public string? Reviews { get; set; }
    public string? Name { get; set; }
    public string? Kind { get; set; }
    public string? Image { get; set; }
    public bool CoffeeOfTheWeek { get; set; }
    public string? Tag { get; set; }
    public CoffeeSticker Sticker { get; set; }
    public string? TasteLead { get; set; }
    public TasteNote[] Notes { get; set; } = [];
    public string? TasteTail { get; set; }
    public int Density { get; set; }
    public int Acidity { get; set; }
    public string? SmallDiscount { get; set; }
    public string? LargeDiscount { get; set; }
    public string? OldPrice { get; set; }
    public string? Price { get; set; }
    public string? Link { get; set; }
}
