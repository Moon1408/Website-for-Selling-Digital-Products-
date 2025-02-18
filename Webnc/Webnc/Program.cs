using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Data;
using System.Security.Claims;
using Webnc.Areas.Admin.Controllers;
using Webnc.Helpers;
using Webnc.Models;
using Webnc.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
// Thêm chuỗi kết nối 
builder.Services.AddDbContext<WebncContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Webnc"));
});
// Thêm này để xuất ra excel 
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
// Sử dụng session lưu cho giỏ hàng 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<KiemTraQuyen>(); // Đăng ký KiemTraQuyen
// ham ChartService de lay 
builder.Services.AddScoped<ChartService>();
builder.Services.AddSession();
// Đăng ký 
// https://docs.automapper.org/en/stable/Dependency-injection.html
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Đăng nhập 
// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-8.0
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/TaiKhoan/DangNhap";
    options.AccessDeniedPath = "/Admin/KiemTraQuyen/TuChoi";
});


builder.Services.AddSingleton<IVnPayService, VnPayService>();
// hàm ProductSevice để kiểm tra số lượng 
builder.Services.AddScoped<ProductService>();
// Authorization setup
builder.Services.AddAuthorization(options =>
{
    //  Thêm chính sách QuanLyP yêu cầu claim loại Role với giá trị "Quản lý". 
    options.AddPolicy("QuanLyP", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Quản lý"));
    // Thêm chính sách NhanVienP yêu cầu claim loại Role với giá trị "Nhân viên".
    options.AddPolicy("NhanVienP", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Nhân viên"));
});

var combinedPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .RequireAssertion(context =>
    // Yêu cầu người dùng phải có vai trò là "Quản lý" hoặc "Nhân viên"
        context.User.IsInRole("Quản lý") || context.User.IsInRole("Nhân viên"))
    .Build();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CombinedPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

builder.Services.AddRazorPages(); 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapRazorPages();


// Sử dụng cho giỏ hàng 
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
            name: "adminArea",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
        );
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapRazorPages();

app.Run();
