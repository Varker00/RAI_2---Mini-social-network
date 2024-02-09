using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Web.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
});


builder.Services.Configure<CookiePolicyOptions>(
options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

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

app.UseAuthorization();


app.MapControllerRoute(
    name: "loginRoute",
    pattern: "Login/{username}",
    defaults: new { controller = "User", action = "Login", id = UrlParameter.Optional });

app.MapControllerRoute(
    name: "delUserRoute",
    pattern: "User/Del/{username}",
    defaults: new { controller = "User", action = "Delete", id = UrlParameter.Optional });

app.MapControllerRoute(
    name: "addFriendRoute",
    pattern: "Friends/Add/{username}",
    defaults: new { controller = "User", action = "AddFriendAction", id = UrlParameter.Optional });

app.MapControllerRoute(
    name: "removeFriendRoute",
    pattern: "Friends/Del/{username}",
    defaults: new { controller = "User", action = "RemoveFriend", id = UrlParameter.Optional });

app.MapControllerRoute(
    name: "addFriendRoute2",
    pattern: "User/Friends/Add/{username}",
    defaults: new { controller = "User", action = "AddFriendAction", id = UrlParameter.Optional });

app.MapControllerRoute(
    name: "removeFriendRoute2",
    pattern: "User/Friends/Del/{username}",
    defaults: new { controller = "User", action = "RemoveFriend", id = UrlParameter.Optional });


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.UseSession();

app.Run();
