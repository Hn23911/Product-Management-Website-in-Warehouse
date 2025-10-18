using WebQuanLySanPhamKhoHang.Data;
using WebQuanLySanPhamKhoHang.Models;
using WebQuanLySanPhamKhoHang.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString
    ("DefaultConnection")));

//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
 .AddDefaultTokenProviders()
 .AddDefaultUI()
 .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication(); ;

app.UseAuthorization();

//app.Use(async (context, next) =>
//{
//    if (context.User.Identity.IsAuthenticated && context.User.IsInRole("Admin"))
//    {
//        var path = context.Request.Path.Value;

//        if (!path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
//        {
//            context.Response.Redirect("/Admin");
//            return;
//        }
//    }
//    else
//    {
//        context.Response.Redirect("/Employee");
//        return;
//    }

//    await next();
//});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "admin",
      pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
      name: "employee",
      pattern: "{area:exists}/{controller=Employee}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    
});

app.MapRazorPages();

app.Run();
