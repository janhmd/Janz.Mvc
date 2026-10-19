var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<Janz.Mvc.Services.AccountApiClient>();
builder.Services.AddScoped<Janz.Mvc.Services.ProductApiClient>();
builder.Services.AddScoped<Janz.Mvc.Services.CartApiClient>();
builder.Services.AddScoped<Janz.Mvc.Services.OrderApiClient>();
builder.Services.AddScoped<Janz.Mvc.Services.AddressApiClient>();

builder.Services.AddHttpClient("JanzApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
