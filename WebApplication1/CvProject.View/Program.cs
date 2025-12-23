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
// Här kopplar vi din specifika User-klass och din DbContext
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Konfigurera lösenordskrav (gör det enkelt för testning)
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.User.RequireUniqueEmail = true; // Kräv unik e-post
})
.AddEntityFrameworkStores<MyAppContext>()
.AddDefaultTokenProviders();

// 3. Konfigurera Cookie-inställningar (Vart oinloggade skickas)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Bra att ha kvar om du använder Identity scaffolding i framtiden

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

// Ordningen här är viktig!
app.UseAuthentication(); // 1. Vem är du?
app.UseAuthorization();  // 2. Vad får du göra?

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();