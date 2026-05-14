using CookBooks.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

//объект, который собирает все настройки перед запуском.
var builder = WebApplication.CreateBuilder(args);

AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(builder.Environment.ContentRootPath, "App_Data"));

// Принудительно указываем, где искать appsettings.json
builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Регистрирует все контроллеры и представления MVC. Теперь ASP.NET знает, что нужно обрабатывать запросы к /Home/Index, /Recipes/Details и т.д.
builder.Services.AddControllersWithViews(); 

//Настраивает подключение к базе данных SQL Server.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CookBooksDb")));


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });
//Настраивает систему входа через cookies.

// После успешного входа создаётся зашифрованная cookie

// При каждом запросе ASP.NET автоматически проверяет эту cookie

// Через 60 минут бездействия пользователь выйдет автоматически

var app = builder.Build();
//Создаёт готовое приложение со всеми настроенными сервисами.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
//Позволяет отдавать статические файлы (CSS, JS, изображения) из папки wwwroot.
app.UseRouting();
//Включает маршрутизацию - определяет, какой контроллер и метод вызывать для каждого URL.
app.UseAuthentication();
// Читает cookie и восстанавливает данные пользователя (User.Identity).
app.UseAuthorization();
// Проверяет права доступа (атрибуты [Authorize]).

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Добавляем тестовые данные
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при добавлении данных: {ex.Message}");
    }
}

app.Run();