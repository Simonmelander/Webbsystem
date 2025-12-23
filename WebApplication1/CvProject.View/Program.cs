using CvProject.Models;
using CvProject.View.Models.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Koppla Databasen
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<MyAppContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Koppla Identity (Inloggningssystemet)
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Inställningar för lösenord och inloggning
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
})
.AddEntityFrameworkStores<MyAppContext>()
.AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; 
    options.AccessDeniedPath = "/Account/AccessDenied";
});


builder.Services.AddControllersWithViews();


builder.Services.AddRazorPages();

var app = builder.Build();

// --- Konfigurera HTTP-pipelinen ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();

app.Run();