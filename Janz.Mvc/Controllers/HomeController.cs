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
            PopularCoffees = PopularCoffees,
            Sets = Sets,
            ReviewCount = "44,828",
            ShopRating = "4.9",
            DeliveryRating = "4.7",
            Reviews = Reviews
        });
    }

    public IActionResult Catalog()
    {
        return View();
    }

    public IActionResult Tea()
    {
        return View();
    }

    public IActionResult Chocolate()
    {
        return View();
    }

    [ActionName("Sets")]
    public IActionResult SetsPage()
    {
        return View("Sets");
    }

    [ActionName("Reviews")]
    public IActionResult ReviewsPage()
    {
        return View("Reviews");
    }

    [HttpGet]
    public IActionResult SignIn()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SignIn(string email, string password, bool remember)
    {
        return View();
    }

    public IActionResult Product(string id)
    {
        if (!string.Equals(id, "floral", StringComparison.OrdinalIgnoreCase))
            return NotFound();

        return View(FloralPage);
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
            Density = 38, Acidity = 72, SmallDiscount = "-15%", LargeDiscount = "-20%",
            OldPrice = "₼32.50", Price = "₼29.90", Link = "/Home/Product/floral"
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

    private static readonly SetCard[] Sets =
    [
        new()
        {
            Category = "Sets", Rating = "4.9", Reviews = "266", Name = "Set with coffee for espresso",
            Image = "set-box.png", Price = "₼143.90",
            Description = "A set of 5 coffee packs for an espresso machine, Turkish coffee and a moka pot"
        },
        new()
        {
            Category = "Sets", Rating = "4.9", Reviews = "92", Name = "Set with coffee for filter",
            Image = "set-box.png", Price = "₼151.90",
            Description = "A set of 5 coffee packs for a drip coffee maker, Turkish coffee, French press, pour-over, AeroPress or a cup"
        },
        new()
        {
            Category = "Sets", Rating = "5", Reviews = "22", Name = "Set with drip bags and a Stojo cup",
            Image = "set-drip.png", Price = "₼139.90",
            Description = "Three packs of drip bags and a foldable Stojo Ink cup in a gift box"
        },
        new()
        {
            Category = "Sets", Rating = "5", Reviews = "1", Name = "Set with coffee and a French press",
            Image = "set-french.png", Price = "₼235.90",
            Description = "A Timemore French press and four packs of bright coffee in a gift box"
        }
    ];

    private static readonly ReviewCard[] Reviews =
    [
        new()
        {
            Name = "Bagdagul", Level = "Newcomer", Initial = "B", Date = "14.08.2026",
            ShopStars = 5, ShopText = "I'm happy with it",
            DeliveryStars = 5, DeliveryText = "Fast",
            Comment = "Everything is as good as always, especially the coffee"
        },
        new()
        {
            Name = "Cavanshir", Level = "Newcomer", Initial = "C", Date = "13.07.2026",
            ShopStars = 5, ShopText = "This was my first time at Janz. I liked everything. The welcome discount is nice too.",
            DeliveryStars = 5, DeliveryText = "Delivery was fast. I'm happy.",
            Comment = "I liked the coffee. So far I'm happy with everything."
        }
    ];

    private static readonly CoffeeCard[] AlsoBuy =
    [
        new()
        {
            Use = "For espresso", Rating = "4.7", Reviews = "3455", Name = "Candy", Kind = "Blend",
            Image = "candy.png", Tag = "Popular", Sticker = CoffeeSticker.Milk,
            TasteLead = "A dense coffee with notes of",
            Notes = [new("dark caramel", "note-dark-caramel"), new("candied fruit", "note-candied"), new("spices", "note-spices")],
            Density = 88, Acidity = 26, LargeDiscount = "-15%", Price = "₼33.20"
        },
        new()
        {
            Use = "For espresso", Rating = "4.7", Reviews = "5512", Name = "Nutty", Kind = "Blend",
            Image = "candy.png", Tag = "Popular", Sticker = CoffeeSticker.Milk,
            TasteLead = "A dense coffee with notes of",
            Notes = [new("roasted nuts", "note-nuts"), new("cookies", "note-caramel"), new("nougat", "note-chocolate")],
            Density = 82, Acidity = 30, LargeDiscount = "-15%", Price = "₼28.80"
        },
        new()
        {
            Use = "For espresso", Rating = "4.7", Reviews = "1175", Name = "Brownie", Kind = "Blend",
            Image = "candy.png", Tag = "Recommended", Sticker = CoffeeSticker.Milk,
            TasteLead = "A dense coffee with notes of",
            Notes = [new("dark chocolate", "note-dark-chocolate"), new("raisins", "note-fruit"), new("nut paste", "note-nuts")],
            Density = 90, Acidity = 16, LargeDiscount = "-13%", Price = "₼32.00"
        }
    ];

    private static readonly CoffeeCard Rwanda = new()
    {
        Use = "For espresso", Rating = "4.5", Reviews = "157", Name = "Rwanda Kagano", Kind = "Microlot",
        Image = "floral.png", Sticker = CoffeeSticker.QGrade,
        TasteLead = "A sweet coffee with notes of",
        Notes = [new("nectarine", "note-peach"), new("bird cherry", "note-cherry"), new("yellow apple", "note-lemon"), new("prune", "note-berries")],
        Density = 52, Acidity = 60, LargeDiscount = "-10%", Price = "₼36.00"
    };

    private static readonly CoffeeCard[] Similar =
    [
        new()
        {
            Use = "For espresso", Rating = "4.6", Reviews = "511", Name = "Fruity", Kind = "Blend",
            Image = "floral.png",
            TasteLead = "A juicy coffee with notes of",
            Notes = [new("orange", "note-orange"), new("milk chocolate", "note-chocolate"), new("dried fruit", "note-fruit")],
            Density = 58, Acidity = 46, LargeDiscount = "-15%", Price = "₼33.20"
        },
        Rwanda,
        new()
        {
            Use = "Omni", Rating = "4.1", Reviews = "137", Name = "Batch Brew Lowcaf", Kind = "50% caffeine, ethyl acetate + funky",
            Image = "floral.png",
            TasteLead = "A bright coffee with notes of",
            Notes = [new("peach", "note-peach"), new("plum", "note-fruit"), new("dark berries", "note-berries"), new("caramel", "note-caramel")],
            Density = 44, Acidity = 70, Price = "₼42.80"
        }
    ];

    private static readonly CoffeeCard[] Viewed =
    [
        Rwanda,
        new()
        {
            Use = "Chocolate and syrups", Rating = "4.7", Reviews = "270", Name = "Marmalade bears with coffee",
            Image = "aroma-kit.png",
            TasteLead = "A pack of coffee marmalade bears",
            Price = "₼6.00"
        },
        new()
        {
            Use = "For filter", Rating = "4.5", Reviews = "2466", Name = "Batch Brew", Kind = "Blend",
            Image = "guatemala.png", Tag = "Popular",
            TasteLead = "A sweet", Notes = [new("fruity", "note-fruity")], TasteTail = "coffee made for batch brew",
            Density = 60, Acidity = 62, LargeDiscount = "-15%", Price = "₼33.90"
        }
    ];

    private static readonly ProductPage FloralPage = new()
    {
        Name = "Floral",
        Kind = "Blend",
        Use = "For espresso",
        Rating = "4.6",
        Reviews = "687",
        CoffeeOfTheWeek = true,
        Tag = "Popular",
        TasteLead = "A light coffee with notes of",
        Notes = [new("flowers", "note-flowers"), new("dark berries", "note-berries"), new("citrus", "note-citrus")],
        Density = 38,
        Acidity = 72,
        OldPrice = "₼32.50",
        Price = "₼29.90",
        DeliveryWhen = "3 – 6 October",
        Roast = "Espresso",
        Region = "Ethiopia",
        Process = "Blend",
        Roaster = "Loring S70 Peregrine",
        Caffeine = "1.24%",
        Summary = "An espresso blend of 100% arabica from two Ethiopian coffees, one natural and one washed, mixed in equal parts.",
        Taste = "A light coffee with notes of flowers, dark berries and citrus.",
        AlsoBuy = AlsoBuy,
        Similar = Similar,
        Viewed = Viewed,
        ViewedSet = Sets[0]
    };

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
