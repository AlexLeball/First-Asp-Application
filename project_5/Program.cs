using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project_5.Data;

var builder = WebApplication.CreateBuilder(args);

// Get connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add DbContext and Identity services 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register only User management 
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
                                       options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); 

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add MVC or Razor Pages services
builder.Services.AddControllersWithViews();

// add razor pages
builder.Services.AddRazorPages();


var app = builder.Build();
// **SEED ADMIN ROLE**
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndAdminAsync(services);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  // Ensure authentication is in place
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=CarList}/{action=Cars}/{id?}");
app.MapRazorPages();

app.Run();

// **Seed Method**
async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string username = "Jacques";
    string adminRole = "Admin";
    string adminEmail = "jacques@example.com";
    string adminPassword = "Password@123"; // Change for production

    // Create Admin role if it does not exist
    var roleExist = await roleManager.RoleExistsAsync(adminRole);
    if (!roleExist)
    {
        var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
        if (!roleResult.Succeeded)
        {
            foreach (var error in roleResult.Errors)
            {
                Console.WriteLine($"Error creating role: {error.Description}");
            }
            return; // Early exit if role creation fails
        }
    }

    // Create Admin user if it does not exist
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = username,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var userResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (!userResult.Succeeded)
        {
            foreach (var error in userResult.Errors)
            {
                Console.WriteLine($"Error creating user: {error.Description}");
            }
            return; // Early exit if user creation fails
        }

        // Assign the role to the user after user creation
        var addToRoleResult = await userManager.AddToRoleAsync(adminUser, adminRole);
        if (!addToRoleResult.Succeeded)
        {
            foreach (var error in addToRoleResult.Errors)
            {
                Console.WriteLine($"Error adding user to role: {error.Description}");
            }
        }
    }
    else
    {
        // If the user already exists, make sure they're assigned to the Admin role
        var isInRole = await userManager.IsInRoleAsync(adminUser, adminRole);
        if (!isInRole)
        {
            var addToRoleResult = await userManager.AddToRoleAsync(adminUser, adminRole);
            if (!addToRoleResult.Succeeded)
            {
                foreach (var error in addToRoleResult.Errors)
                {
                    Console.WriteLine($"Error adding user to role: {error.Description}");
                }
            }
        }
    }
}
