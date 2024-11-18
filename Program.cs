using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // หน้า Login เมื่อไม่มีการ Authenticated
        options.LogoutPath = "/Logout"; // หน้า Logout
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // อายุของ Cookie
    });

builder.Services.AddAuthorization();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseAuthentication(); // เพิ่ม Middleware สำหรับ Authentication
app.UseAuthorization();

app.MapRazorPages();
app.Run();
