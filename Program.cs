using EduManage.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    // Increase session timeout to reduce antiforgery token expiry during admin work
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

// If antiforgery validation fails it results in a 400 response. Redirect
// admin 400s back to the Add page so the admin gets a fresh form/token
app.UseStatusCodePages(async context =>
{
    var resp = context.HttpContext.Response;
    if (resp.StatusCode == 400)
    {
        var req = context.HttpContext.Request;
        if (req.Path.StartsWithSegments("/Admin", StringComparison.OrdinalIgnoreCase))
        {
            resp.Redirect("/Admin/AddStudent");
            return;
        }
        resp.Redirect("/");
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();