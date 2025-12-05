using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechNews.Data;
using TechNews.Models;
using React.AspNet;
using JavaScriptEngineSwitcher.Jint;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Підключення БД Користувачів (Identity)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Підключення БД Новин
var newsConnectionString = builder.Configuration.GetConnectionString("NewsConnection") 
    ?? throw new InvalidOperationException("Connection string 'NewsConnection' not found.");
builder.Services.AddDbContext<NewsContext>(options =>
    options.UseSqlite(newsConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Налаштування Identity з ролями
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddReact();
builder.Services.AddJsEngineSwitcher(options => options.DefaultEngineName = JintJsEngine.EngineName)
       .AddJint();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/PageNotFound");

app.UseReact(config => { });
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    var newsContext = services.GetRequiredService<NewsContext>();
    var userContext = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    newsContext.Database.EnsureCreated();
    userContext.Database.EnsureCreated();

    // Створення ролей (Admin + Editor)
    string[] roles = new[] { "Admin", "Editor" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    try 
    {
        await DbSeeder.SeedData(services, newsContext, userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Помилка при заповненні бази даних");
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
