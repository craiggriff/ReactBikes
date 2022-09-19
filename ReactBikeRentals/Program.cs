using Microsoft.EntityFrameworkCore;
using ReactBikes.Data;
using Microsoft.AspNetCore.Identity;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ReactBikesContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReactBikesContext") ?? throw new InvalidOperationException("Connection string 'ReactBikesContext' not found.")));

builder.Services.AddIdentity<ReactBikesUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;

})
           .AddDefaultTokenProviders()
           .AddDefaultUI()
           .AddRoles<IdentityRole>()
           .AddEntityFrameworkStores<ReactBikesContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

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
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    try
    {
        var context = services.GetRequiredService<ReactBikesContext>();
        var userManager = services.GetRequiredService<UserManager<ReactBikesUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await ReactBikesDBInitialize.AddRolesAsync(userManager, roleManager);
        await ReactBikesDBInitialize.AddManagerAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {

    }
}

app.Run();
