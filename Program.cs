using GooseShop.Components;
using GooseShop.Data;
using GooseShop.Models;
using GooseShop.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. НАЛАШТУВАННЯ БАЗИ ДАНИХ (SQLite)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=GooseShop.db";

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// 2. РЕЄСТРАЦІЯ СЕРВІСІВ (DI)
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<NovaPoshtaService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<AuthService>();

// 3. НАЛАШТУВАННЯ BLAZOR
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true; // Для виведення детальних помилок у консоль
    });

builder.Services.AddHttpClient();

var app = builder.Build();

// 4. КОНФІГУРАЦІЯ HTTP-КОНВЕЄРА
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

// Налаштування для коректного відображення 3D-молей (.glb)
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".glb"] = "model/gltf-binary";
provider.Mappings[".gltf"] = "model/gltf+json";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

// 5. SEED DATA - НАПОВНЕННЯ БАЗИ ПРИ СТАРТІ
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Автоматично накочує міграції (створює таблиці, якщо їх нема)
    context.Database.Migrate();

    // Видалення тестового юзера (якщо потрібно)
    var userToDelete = context.Users.FirstOrDefault(u => u.Email == "velihorsee@gmail.com");
    if (userToDelete != null)
    {
        context.Users.Remove(userToDelete);
        context.SaveChanges();
    }

    // Створення головного адміна
    if (!context.Users.Any(u => u.Email == "sales.gooseinua@gmail.com"))
    {
        context.Users.Add(new User
        {
            FullName = "Admin",
            Email = "sales.gooseinua@gmail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("12345"),
            Role = "Admin"
        });
        context.SaveChanges();
    }

    // Створення початкової категорії та товару (якщо база порожня)
    if (!context.Categories.Any())
    {
        var cat = new Category { Name = "Чашки" };
        context.Categories.Add(cat);
        context.SaveChanges();

        var prod = new Product
        {
            Name = "Гусь-Гангстер",
            Description = "Класична чашка з гусаком, який знає собі ціну.",
            BasePrice = 250,
            CategoryId = cat.Id,
            ImageUrl = "images/mug.jpg" // Переконайтеся, що файл є у wwwroot/images/
        };
        context.Products.Add(prod);
        context.SaveChanges();
    }

    // Додавання конструктора для першого товару
    var mainProduct = context.Products.FirstOrDefault();
    if (mainProduct != null)
    {
        if (!context.ProductConstructors.Any(c => c.ProductId == mainProduct.Id))
        {
            context.ProductConstructors.Add(new ProductConstructor
            {
                ProductId = mainProduct.Id,
                ModelPath = "models/cup.glb",
                Type = ConstructorType.Cup
            });
            context.SaveChanges();
        }

        // Додавання варіацій (Ціни беремо звідси!)
        if (!context.ProductVariants.Any(v => v.ProductId == mainProduct.Id))
        {
            context.ProductVariants.AddRange(
                new ProductVariant { ProductId = mainProduct.Id, Size = "330", Color = "white", Price = 250, Title = "330мл Біла", StockQuantity = 20 },
                new ProductVariant { ProductId = mainProduct.Id, Size = "330", Color = "black", Price = 270, Title = "330мл Чорна", StockQuantity = 15 },
                new ProductVariant { ProductId = mainProduct.Id, Size = "440", Color = "white", Price = 320, Title = "440мл Біла", StockQuantity = 10 }
            );
            context.SaveChanges();
        }
    }
}

// 6. МАРШРУТИЗАЦІЯ BLAZOR
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();