using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.Models;

namespace Janz.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View(new HomeIndexViewModel
        {
            NewCoffees = NewCoffees,
            PopularCoffees = PopularCoffees
        });
    }

    private static readonly CoffeeCard[] NewCoffees =
    [
        new()
        {
            Use = "For filter", Rating = "5", Reviews = "67", Name = "Indonesia Sumatra Strawberry Karo", Kind = "Infused",
            Image = "guatemala.png", CoffeeOfTheWeek = true, Tag = "New", Sticker = CoffeeSticker.QGrade,
            TasteLead = "A bright coffee with notes of",
            Notes = [new("strawberry", "note-strawberry"), new("cherry", "note-cherry"), new("cream caramel", "note-caramel"), new("milk chocolate", "note-chocolate")],
            Density = 58, Acidity = 84, SmallDiscount = "-15%", LargeDiscount = "-20%", OldPrice = "₼29.70", Price = "₼24.90"
        },
        new()
        {
            Use = "For espresso", Rating = "0", Reviews = "6", Name = "Costa Rica Reposado", Kind = "Natural",
            Image = "floral.png", CoffeeOfTheWeek = true, Tag = "New", Sticker = CoffeeSticker.QGrade,
            TasteLead = "A bright coffee with notes of",
            Notes = [new("tangerine", "note-tangerine"), new("mango", "note-mango"), new("apricot", "note-apricot"), new("lemon drops", "note-lemon")],
            Density = 70, Acidity = 52, SmallDiscount = "-15%", LargeDiscount = "-20%", OldPrice = "₼28.50", Price = "₼24.20"
        },
        new()
        {
            Use = "For filter", Rating = "0", Reviews = "2", Name = "Costa Rica Vista Al Santo", Kind = "Natural",
            Image = "guatemala.png", CoffeeOfTheWeek = true, Tag = "New", Sticker = CoffeeSticker.QGrade,
            TasteLead = "A bright coffee with notes of",
            Notes = [new("passion fruit", "note-passion"), new("peach", "note-peach"), new("raspberry", "note-raspberry"), new("dark chocolate", "note-dark-chocolate")],
            Density = 50, Acidity = 72, SmallDiscount = "-15%", LargeDiscount = "-20%", OldPrice = "₼29.50", Price = "₼24.90"
        },
        new()
        {
            Use = "For espresso", Rating = "0", Reviews = "4", Name = "Indonesia Sumatra Karo Funky", Kind = "Natural funky",
            Image = "floral.png", CoffeeOfTheWeek = true, Tag = "New", Sticker = CoffeeSticker.QGrade,
            TasteLead = "A bright coffee with notes of",
            Notes = [new("pineapple", "note-pineapple"), new("passion fruit", "note-passion"), new("berry vermouth", "note-vermouth"), new("dark chocolate", "note-dark-chocolate")],
            Density = 72, Acidity = 66, SmallDiscount = "-15%", LargeDiscount = "-20%", OldPrice = "₼29.00", Price = "₼24.70"
        }
    ];

    private static readonly CoffeeCard[] PopularCoffees =
    [
        new()
        {
            Use = "For espresso", Rating = "4.7", Reviews = "3454", Name = "Candy", Kind = "Blend",
            Image = "candy.png", Tag = "Popular", Sticker = CoffeeSticker.Milk,
            TasteLead = "A dense coffee with notes of",
            Notes = [new("dark caramel", "note-dark-caramel"), new("candied fruit", "note-candied"), new("spices", "note-spices")],
            Density = 88, Acidity = 26, LargeDiscount = "-15%", Price = "₼33.20"
        },
        new()
        {
            Use = "For espresso", Rating = "4.7", Reviews = "6073", Name = "Ethiopia Yirgacheffe", Kind = "Natural",
            Image = "floral.png", Tag = "Popular",
            TasteLead = "A sweet coffee with notes of",
            Notes = [new("dark berries", "note-berries"), new("citrus", "note-citrus"), new("milk chocolate", "note-chocolate")],
            Density = 54, Acidity = 50, LargeDiscount = "-15%", Price = "₼31.20"
        },
        new()
        {
            Use = "For espresso", Rating = "4.7", Reviews = "6715", Name = "Brazil Cerrado", Kind = "Natural",
            Image = "floral.png", Tag = "Popular",
            TasteLead = "A dense coffee with notes of",
            Notes = [new("roasted nuts", "note-nuts"), new("chocolate", "note-chocolate"), new("caramel", "note-caramel")],
            Density = 70, Acidity = 34, LargeDiscount = "-15%", Price = "₼28.80"
        },
        new()
        {
            Use = "For espresso", Rating = "4.6", Reviews = "687", Name = "Floral", Kind = "Blend",
            Image = "floral.png", CoffeeOfTheWeek = true, Tag = "Popular",
            TasteLead = "A light coffee with notes of",
            Notes = [new("flowers", "note-flowers"), new("dark berries", "note-berries"), new("citrus", "note-citrus")],
            Density = 38, Acidity = 72, SmallDiscount = "-15%", LargeDiscount = "-20%", OldPrice = "₼32.50", Price = "₼29.90"
        },
        new()
        {
            Use = "For filter", Rating = "4.6", Reviews = "2031", Name = "Colombia Magdalena", Kind = "Washed",
            Image = "guatemala.png", Tag = "Popular",
            TasteLead = "A juicy coffee with notes of",
            Notes = [new("red apple", "note-apple"), new("dark grapes", "note-grapes"), new("cocoa", "note-cocoa")],
            Density = 56, Acidity = 80, LargeDiscount = "-15%", Price = "₼33.20"
        },
        new()
        {
            Use = "For filter", Rating = "4.6", Reviews = "2856", Name = "Brazil Iguassu", Kind = "Natural",
            Image = "guatemala.png", Tag = "Popular",
            TasteLead = "A sweet coffee with notes of",
            Notes = [new("peanut", "note-peanut"), new("cocoa powder", "note-cocoa"), new("lemon zest", "note-lemon")],
            Density = 50, Acidity = 42, LargeDiscount = "-15%", Price = "₼29.90"
        },
        new()
        {
            Use = "For filter", Rating = "4.5", Reviews = "2463", Name = "Batch Brew", Kind = "Blend",
            Image = "guatemala.png", Tag = "Popular",
            TasteLead = "A sweet", Notes = [new("fruity", "note-fruity")], TasteTail = "coffee made for batch brew",
            Density = 60, Acidity = 62, LargeDiscount = "-15%", Price = "₼33.90"
        }
    ];

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
