using System.Diagnostics;
using Janz.Mvc.Models;
using Janz.Mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.Controllers;

public class HomeController : Controller
{
    private const string TokenCookie = "janz_token";
    private const string NameCookie = "janz_name";

    private readonly AccountApiClient _accountApi;

    public HomeController(AccountApiClient accountApi)
    {
        _accountApi = accountApi;
    }

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string name, string email, string password, string password2)
    {
        if (password != password2)
        {
            ViewBag.Error = "Passwords do not match.";
            ViewBag.Name = name;
            ViewBag.Email = email;
            return View();
        }

        var result = await _accountApi.RegisterAsync(name ?? string.Empty, email ?? string.Empty, password ?? string.Empty, password2 ?? string.Empty);
        if (!result.Success)
        {
            ViewBag.Error = string.Join(" ", result.Errors);
            ViewBag.Name = name;
            ViewBag.Email = email;
            return View();
        }

        WriteAuthCookies(result, remember: true);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult SignIn()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn(string email, string password, bool remember)
    {
        var result = await _accountApi.LoginAsync(email ?? string.Empty, password ?? string.Empty);
        var ajax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

        if (!result.Success)
        {
            if (ajax)
            {
                return BadRequest(new { errors = result.Errors });
            }

            ViewBag.Error = string.Join(" ", result.Errors);
            ViewBag.Email = email;
            return View();
        }

        WriteAuthCookies(result, remember);

        if (ajax)
        {
            return Ok(new { ok = true });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult LogOut()
    {
        Response.Cookies.Delete(TokenCookie);
        Response.Cookies.Delete(NameCookie);
        return RedirectToAction(nameof(Index));
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

    private void WriteAuthCookies(AuthCallResult result, bool remember)
    {
        var life = new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = Request.IsHttps
        };

        if (remember)
        {
            life.Expires = DateTimeOffset.UtcNow.AddDays(7);
        }

        Response.Cookies.Append(TokenCookie, result.Token, life);
        Response.Cookies.Append(NameCookie, result.FullName, life);
    }
}
