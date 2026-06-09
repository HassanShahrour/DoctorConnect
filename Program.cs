using DoctorConnect.Extensions;
using DoctorConnect.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInterfaceScopeServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = new[] { "SuperAdmin", "Admin", "Doctor", "Patient" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        //To be removed in production - only for testing purposes
        var superEmail = "superadmin@doctorconnect.com";
        var super = await userManager.FindByEmailAsync(superEmail);
        if (super == null)
        {
            super = new ApplicationUser
            {
                UserName = superEmail,
                Email = superEmail,
                EmailConfirmed = true,
                Address = "Beirut, Lebanon",
                FirstName = "Hassan",
                LastName = "Shahrour",
                Gender = "Male"
            };
            await userManager.CreateAsync(super, "SuperAdmin@123");
            await userManager.AddToRoleAsync(super, "SuperAdmin");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");

app.MapRazorPages();

app.Run();
