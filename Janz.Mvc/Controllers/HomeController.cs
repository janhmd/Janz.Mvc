using System.Diagnostics;
using Janz.Mvc.Models;
using Janz.Mvc.Services;
using Janz.Mvc.ViewModels.Cart;
using Janz.Mvc.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.Controllers;

public class HomeController : Controller
{
    private const string TokenCookie = "janz_token";
    private const string NameCookie = "janz_name";

    private readonly AccountApiClient _accountApi;
    private readonly ProductApiClient _productApi;
    private readonly CartApiClient _cartApi;

    public HomeController(AccountApiClient accountApi, ProductApiClient productApi, CartApiClient cartApi)
    {
        _accountApi = accountApi;
        _productApi = productApi;
        _cartApi = cartApi;
    }

    public async Task<IActionResult> Index()
    {
        var coffee = await _productApi.GetByCategoryAsync("coffee");
        var sets = await _productApi.GetByCategoryAsync("sets");
        return View(new HomeIndexVM
        {
            Coffee = coffee,
            Sets = sets
        });
    }

    public async Task<IActionResult> Catalog()
    {
        return View(await _productApi.GetByCategoryAsync("coffee"));
    }

    public async Task<IActionResult> Tea()
    {
        return View(await _productApi.GetByCategoryAsync("tea"));
    }

    public async Task<IActionResult> Chocolate()
    {
        return View(await _productApi.GetByCategoryAsync("chocolate"));
    }

    public async Task<IActionResult> Sets()
    {
        return View(await _productApi.GetByCategoryAsync("sets"));
    }

    public IActionResult Reviews()
    {
        return View();
    }

    public async Task<IActionResult> Product(int? id)
    {
        ProductUIVM? product = null;
        if (id.HasValue)
        {
            product = await _productApi.GetByIdAsync(id.Value);
        }
        else
        {
            product = (await _productApi.GetByCategoryAsync("coffee")).FirstOrDefault();
        }

        if (product is null)
        {
            return NotFound();
        }

        var related = (await _productApi.GetByCategoryAsync(product.CategorySlug))
            .Where(m => m.Id != product.Id)
            .Take(3)
            .ToList();

        return View(new ProductPageVM
        {
            Product = product,
            Related = related
        });
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

    public async Task<IActionResult> Cart()
    {
        var cart = new CartUIVM();
        var token = Request.Cookies[TokenCookie];
        if (!string.IsNullOrEmpty(token))
        {
            var result = await _cartApi.GetAsync(token);
            if (result.Success)
            {
                cart = result.Cart;
            }
        }

        var inCart = cart.Items.Select(m => m.ProductId).ToList();
        var products = await _productApi.GetAllAsync();
        return View(new CartPageVM
        {
            Cart = cart,
            Suggestions = products.Where(m => !inCart.Contains(m.Id)).Take(3).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int productId, int quantity)
    {
        var token = Request.Cookies[TokenCookie];
        if (string.IsNullOrEmpty(token))
        {
            return IsAjax() ? Unauthorized() : RedirectToAction(nameof(SignIn));
        }

        var result = await _cartApi.AddAsync(token, productId, quantity);
        return CartResponse(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCart(int id, int quantity)
    {
        var token = Request.Cookies[TokenCookie];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction(nameof(SignIn));
        }

        var result = await _cartApi.UpdateAsync(token, id, quantity);
        return CartResponse(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromCart(int id)
    {
        var token = Request.Cookies[TokenCookie];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction(nameof(SignIn));
        }

        var result = await _cartApi.RemoveAsync(token, id);
        return CartResponse(result);
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

    private IActionResult CartResponse(CartCallResult result)
    {
        if (result.Unauthorized)
        {
            Response.Cookies.Delete(TokenCookie);
            Response.Cookies.Delete(NameCookie);
            return IsAjax() ? Unauthorized() : RedirectToAction(nameof(SignIn));
        }

        if (IsAjax())
        {
            return result.Success
                ? Ok(new { count = result.Cart.Count })
                : BadRequest(new { errors = result.Errors });
        }

        if (!result.Success)
        {
            TempData["CartError"] = string.Join(" ", result.Errors);
        }

        return RedirectToAction(nameof(Cart));
    }

    private bool IsAjax()
    {
        return string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
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
