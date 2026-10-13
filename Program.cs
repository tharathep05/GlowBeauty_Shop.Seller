using Microsoft.EntityFrameworkCore;
using GlowBeauty_Shop.Seller.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// เปิดใช้งาน Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// แก้ไข Error 19 ด้วย EnableRetryOnFailure (ระบบลองเชื่อมต่อใหม่เมื่อ Connection หลุด)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

var app = builder.Build();

// รัน Seed Data สร้าง DB และบัญชีผู้ขาย
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "เกิดข้อผิดพลาดในการเริ่มต้นฐานข้อมูล");
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

app.UseSession();

app.UseAuthorization();

// ตั้งค่าให้หน้าแรกบังคับไปที่หน้าเข้าสู่ระบบ (Account/Login)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();