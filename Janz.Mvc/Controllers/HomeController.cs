using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Janz.Mvc.Models;

namespace Janz.Mvc.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
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

    public IActionResult Sets()
    {
        return View();
    }

    public IActionResult Reviews()
    {
        return View();
    }

    public IActionResult Product()
    {
        return View();
    }

    public IActionResult Community()
    {
        return View();
    }

    public IActionResult Journal()
    {
        return View();
    }

    public IActionResult Article()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    public IActionResult SignIn()
    {
        return View();
    }

    public IActionResult Wishlist()
    {
        return View();
    }

    public IActionResult Cart()
    {
        return View();
    }

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
