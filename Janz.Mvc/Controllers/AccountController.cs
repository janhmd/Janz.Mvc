using Janz.Mvc.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Janz.Mvc.Controllers
{
    // Qeydiyyat, giriş və çıxış. Giriş cookie-dədir: JWT onun içində şifrələnmiş saxlanır, JS tokeni görə bilmir
    public class AccountController : ApiControllerBase
    {
        // API-dəki Jwt:ExpireDays ilə eyni: cookie tokendən əvvəl bitməsin, sonra da qalmasın
        private const int TokenDays = 7;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginVM
            {
                ReturnUrl = returnUrl
            });
        }

        // Həm səhifədəki formadan, həm də header-dəki pəncərədən (fetch, X-Requested-With) gəlir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            var password = model.Password;
            model.Password = string.Empty; // şifrə forma geri qaytarılmasın

            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrEmpty(password))
            {
                return LoginFailed(model, new[] { "Enter your email and password." });
            }

            var (result, errors, _) = await PostAsync<LoginResponseVM>("api/account/login", new
            {
                email = model.Email.Trim(),
                password
            });

            if (result is null || string.IsNullOrEmpty(result.Token))
            {
                return LoginFailed(model, errors);
            }

            await SignInAsync(result, model.Remember);

            if (IsAjax())
            {
                return Ok(new { ok = true });
            }

            // Əvvəl getmək istədiyi yerə (ReturnUrl), yoxdursa ana səhifəyə
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new RegisterVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            var password = model.Password;
            var repeatPassword = model.RepeatPassword;
            model.Password = string.Empty;
            model.RepeatPassword = string.Empty;

            if (password != repeatPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return View(model);
            }

            var (result, errors, _) = await PostAsync<LoginResponseVM>("api/account/register", new
            {
                fullName = model.FullName?.Trim() ?? string.Empty,
                email = model.Email?.Trim() ?? string.Empty,
                password = password ?? string.Empty,
                repeatPassword = repeatPassword ?? string.Empty
            });

            if (result is null || string.IsNullOrEmpty(result.Token))
            {
                foreach (var error in errors)
                    ModelState.AddModelError(string.Empty, error);

                return View(model);
            }

            // Qeydiyyatdan sonra ayrıca giriş lazım deyil
            await SignInAsync(result, remember: true);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private IActionResult LoginFailed(LoginVM model, string[] errors)
        {
            if (IsAjax())
            {
                return BadRequest(new { errors });
            }

            foreach (var error in errors)
                ModelState.AddModelError(string.Empty, error);

            return View(model);
        }

        private async Task SignInAsync(LoginResponseVM login, bool remember)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, login.Email),
                new(ClaimTypes.Name, string.IsNullOrWhiteSpace(login.FullName) ? login.Email : login.FullName)
            };
            if (!string.IsNullOrEmpty(login.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, login.Role));
            }

            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

            var properties = new AuthenticationProperties
            {
                IsPersistent = remember,                         // "Remember password" yoxdursa brauzer bağlananda giriş bitir
                AllowRefresh = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(TokenDays)
            };

            // JWT cookie-nin içində (şifrələnmiş) saxlanır; BearerTokenHandler onu oradan oxuyur
            properties.StoreTokens(new[]
            {
                new AuthenticationToken
                {
                    Name = "access_token",
                    Value = login.Token
                }
            });

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        }
    }
}
