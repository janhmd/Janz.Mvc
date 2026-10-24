using Janz.Mvc.Handlers;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Qiymət kimi onluq ədədlər formadan nöqtə ilə gəlir (29.90): kompüterin dil ayarından asılı olmasın (API-də də belədir)
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<BearerTokenHandler>();

// API-yə gedən sorğulara cookie-dəki JWT avtomatik əlavə olunur
builder.Services.AddHttpClient("JanzApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
})
.AddHttpMessageHandler<BearerTokenHandler>();

// Giriş cookie-dədir (JWT onun içində, şifrələnmiş saxlanır; JS tokeni görə bilmir)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // giriş tələb edən səhifə
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.Cookie.Name = "Janz.Auth";
        options.Cookie.HttpOnly = true;
        // Production-da yalnız HTTPS; lokal işləmədə HTTP profili ilə də giriş işləsin
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.SlidingExpiration = false; // cookie müddəti JWT-nin bitmə vaxtı ilə eynidir
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// 404, 403 və s. (məs. mövcud olmayan id) boş səhifə əvəzinə xəta səhifəsini göstərsin
app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Admin panel: /Admin/{controller}/{action} (Areas/Admin). Default route-dan əvvəl olmalıdır
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
