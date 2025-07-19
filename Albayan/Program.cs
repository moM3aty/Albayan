using Microsoft.EntityFrameworkCore;
using Albayan.Areas.Admin.Data;
using Albayan.Models;
using Microsoft.AspNetCore.Identity;
using Albayan.Services;
using System;
using Albayan.Data;

var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PlatformDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<PlatformDbContext>();
builder.Services.ConfigureApplicationCookie(options => { options.LoginPath = "/Account/LoginAdmin"; });
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IFileService, FileService>();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/admin/Account/LoginAdmin";


    options.AccessDeniedPath = "/admin/Account/LoginAdmin";
});


builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IFileService, FileService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbSeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

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
    name: "AdminArea",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
