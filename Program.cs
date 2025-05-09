using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentSync.Data;
using StudentSync.Interfaces;
using StudentSync.Models;
using StudentSync.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the unified ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity + Roles using the same database
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
        options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Register custom services
builder.Services.AddScoped<IStudent, StudentRepo>();
builder.Services.AddScoped<IDBInitializer, DBInitializerRepo>();
builder.Services.AddScoped<IConsumer, ConsumerRepo>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed roles, admin, and student data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Create roles
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Student", "Consumer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Create default admin user
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    string email = "admin@gmail.com";
    string password = "Test!1234";

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new IdentityUser
        {
            UserName = email,
            Email = email
        };

        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "Admin");
    }

    // Seed Students and Consumer
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    var dbInitializer = services.GetRequiredService<IDBInitializer>();
    dbInitializer.Initialize(dbContext);
}


app.Run();
