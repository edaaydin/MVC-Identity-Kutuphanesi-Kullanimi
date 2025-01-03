using _11_Identity.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// sql yapılandırması.

builder.Services.AddDbContext<Context>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultStr")));

// identity yapılandırması.

builder.Services.AddIdentity<AppUser, AppRole>
    (
    opt =>
    {
        opt.User.RequireUniqueEmail = true;
        opt.Password.RequiredLength = 3; // en az 3 karakter olsun.
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequireDigit = false;
        opt.Password.RequireLowercase = false;
        opt.Password.RequireUppercase = false;
    }
    ).AddEntityFrameworkStores<Context>();

// Cookie yapılanması

builder.Services.ConfigureApplicationCookie
    (
    opt =>
    {
        opt.Cookie.Name = "UserCookie";
        opt.ExpireTimeSpan = TimeSpan.FromMinutes(3); // 3 dk boyunca beni hatırla (Gecerlilik suresi)
        opt.SlidingExpiration = true; // bir 3 daha vereyim mi diye sorar.yani zaman asımıdır. (Hareket varsa sure bir o kadar daha uzatılsın.)

    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // kimlik dogrulama / Sen kimsin ?

app.UseAuthorization(); // yetki

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
