using GooseShop.Components;
using GooseShop.Data;
using GooseShop.Models;
using GooseShop.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Налаштування бази даних
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=GooseShop.db"; // Запасний варіант, якщо в appsettings порожньо
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddDbContext<AppDbContext>(options =>
    //options.UseSqlite(connectionString));

// 2. Реєстрація сервісу
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CartService>(); // Scoped означає "один кошик на одного відвідувача"
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<NovaPoshtaService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<AuthService>();
// 3. Налаштування Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true; // Додай цей рядок
    });


builder.Services.AddHttpClient();
var app = builder.Build();

// --- ЦЬОГО БЛОКУ У ВАС НЕ ВИСТАЧАЛО ---

// Налаштування обробки помилок та статичних файлів (CSS/JS)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

// Налаштування маршрутизації для Blazor компонентів
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ---------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // 1. СТВОРЮЄМО БАЗУ ТА ТАБЛИЦІ (це найголовніше)
    db.Database.EnsureCreated();

    // 2. Тепер вже можна працювати з даними
    var userToDelete = db.Users.FirstOrDefault(u => u.Email == "velihorsee@gmail.com");
    if (userToDelete != null)
    {
        db.Users.Remove(userToDelete);
        db.SaveChanges();
    }

    // 3. Створення адміна (як у тебе в коді)
    if (!db.Users.Any(u => u.Email == "sales.gooseinua@gmail.com"))
    {
        db.Users.Add(new User
        {
            FullName = "Admin",
            Email = "sales.gooseinua@gmail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("12345"),
            Role = "Admin"
        });
        db.SaveChanges();
    }
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Створює базу та таблиці, якщо їх ще немає
    db.Database.EnsureCreated();

    // Додаємо зв'язок конструктора для товару з ID 1 (наприклад, ваша чашка)
    if (!db.ProductConstructors.Any(c => c.ProductId == 1))
    {
        db.ProductConstructors.Add(new ProductConstructor
        {
            ProductId = 1,
            ModelPath = "models/cup.glb",
            Type = ConstructorType.Cup
        });
        db.SaveChanges();
    }
}
var provider = new FileExtensionContentTypeProvider();
// Чітко вказуємо серверу, що таке .glb файли
provider.Mappings[".glb"] = "model/gltf-binary";
provider.Mappings[".gltf"] = "model/gltf+json";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});
app.Run();