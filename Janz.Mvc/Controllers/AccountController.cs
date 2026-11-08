using Janz.Mvc.Helpers;
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

            // Əvvəl getmək istədiyi yerə (ReturnUrl), yoxdursa işçi admin panelinə, müştəri ana səhifəyə
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl);
            }

            return LocalRedirect(RoleRedirect.HomeUrl(RolePrincipal(result.Role)));
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

            var (result, errors, _) = await PostAsync<AccountEmailVM>("api/account/register", new
            {
                fullName = model.FullName?.Trim() ?? string.Empty,
                email = model.Email?.Trim() ?? string.Empty,
                password = password ?? string.Empty,
                repeatPassword = repeatPassword ?? string.Empty
            });

            if (result is null)
            {
                foreach (var error in errors)
                    ModelState.AddModelError(string.Empty, error);

                return View(model);
            }

            // Qeydiyyat emailə gələn 4 rəqəmli kod yazılandan sonra tamamlanır
            return RedirectToAction(nameof(Verify), new { email = result.Email });
        }

        // Kodun yazıldığı səhifə: kodu yenidən göndərmək də buradadır
        [HttpGet]
        public IActionResult Verify(string? email)
        {
            return View(new VerifyVM { Email = email ?? string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(VerifyVM model)
        {
            model.Code = model.Code?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(model.Email) || model.Code.Length != 4 || !model.Code.All(char.IsDigit))
            {
                ModelState.AddModelError(string.Empty, "Enter the 4 digits from the email.");
                model.Code = string.Empty;
                return View(model);
            }

            var (result, errors, _) = await PostAsync<LoginResponseVM>("api/account/confirm-email", new { email = model.Email.Trim(), code = model.Code });
            if (result is null || string.IsNullOrEmpty(result.Token))
            {
                foreach (var error in errors)
                    ModelState.AddModelError(string.Empty, error);
                model.Code = string.Empty;
                return View(model);
            }

            await SignInAsync(result, remember: true);
            TempData["Toast"] = "Your email is confirmed. Welcome to Janz!";
            return LocalRedirect(RoleRedirect.HomeUrl(RolePrincipal(result.Role)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendCode(VerifyVM model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError(string.Empty, "Enter your email.");
                return View(nameof(Verify), model);
            }

            var (success, errors, _) = await PostAsync("api/account/resend-confirmation", new { email = model.Email.Trim() });
            if (!success)
            {
                foreach (var error in errors)
                    ModelState.AddModelError(string.Empty, error);
                return View(nameof(Verify), model);
            }

            TempData["AccountDone"] = "If this email is waiting for confirmation, we sent a new code.";
            return RedirectToAction(nameof(Verify), new { email = model.Email.Trim() });
        }

        [HttpGet]
        public IActionResult ForgotPassword(string? email)
        {
            return View(new AccountEmailVM { Email = email ?? string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(AccountEmailVM model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError(string.Empty, "Enter your email.");
                return View(model);
            }

            var (success, errors, _) = await PostAsync("api/account/forgot-password", new { email = model.Email.Trim() });
            if (!success)
            {
                foreach (var error in errors)
                    ModelState.AddModelError(string.Empty, error);
                return View(model);
            }

            // Email qeydiyyatdadırmı, bildirilmir
            TempData["AccountDone"] = "If an account with this email exists, we sent a link to reset the password.";
            return RedirectToAction(nameof(ForgotPassword), new { email = model.Email.Trim() });
        }

        [HttpGet]
        public IActionResult ResetPassword(string? userId, string? token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            return View(new ResetPasswordVM { UserId = userId, Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            var (result, errors, _) = await PostAsync<AccountEmailVM>("api/account/reset-password", new
            {
                userId = model.UserId,
                token = model.Token,
                email = model.Email?.Trim() ?? string.Empty,
                password = model.Password ?? string.Empty,
                repeatPassword = model.RepeatPassword ?? string.Empty
            });

            model.Password = string.Empty;
            model.RepeatPassword = string.Empty;
            if (result is null)
            {
                foreach (var error in errors)
                    ModelState.AddModelError(string.Empty, error);
                return View(model);
            }

            TempData["AccountDone"] = "Your password was changed. Sign in with the new one.";
            return RedirectToAction(nameof(Login));
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

        // Giriş yenicə edildiyi üçün User hələ köhnədir: yönləndirmə API-dən gələn rola görə seçilir
        private static ClaimsPrincipal RolePrincipal(string? role)
        {
            var identity = new ClaimsIdentity("login");
            if (!string.IsNullOrEmpty(role))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            return new ClaimsPrincipal(identity);
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
