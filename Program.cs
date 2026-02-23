using GooseShop.Components;
using GooseShop.Data;
using GooseShop.Models;
using GooseShop.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. НАЛАШТУВАННЯ БАЗИ ДАНИХ
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=GooseShop.db";

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// 2. РЕЄСТРАЦІЯ СЕРВІСІВ
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
        options.DetailedErrors = true;
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

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".glb"] = "model/gltf-binary";
provider.Mappings[".gltf"] = "model/gltf+json";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

// 5. SEED DATA - НАПОВНЕННЯ БАЗИ
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Автоматично накочує міграції
    context.Database.Migrate();

    // Створення адміна
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

    // --- КАТЕГОРІЯ: ЧАШКИ ---
    if (!context.Categories.Any(c => c.Name == "Чашки"))
    {
        var cupCat = new Category { Name = "Чашки" };
        context.Categories.Add(cupCat);
        context.SaveChanges();

        var cupProd = new Product
        {
            Name = "Гусь-Гангстер",
            Description = "Класична чашка для справжнього генгу.",
            BasePrice = 250,
            CategoryId = cupCat.Id,
            ImageUrl = "images/mug.jpg"
        };
        context.Products.Add(cupProd);
        context.SaveChanges();

        context.ProductVariants.AddRange(
            new ProductVariant { ProductId = cupProd.Id, Size = "330", Color = "white", Price = 250, Title = "330мл Біла", StockQuantity = 20 },
            new ProductVariant { ProductId = cupProd.Id, Size = "440", Color = "white", Price = 320, Title = "440мл Біла", StockQuantity = 10 }
        );
        context.SaveChanges();
    }

    // --- КАТЕГОРІЯ: ФУТБОЛКИ (ТВІЙ ЗАПИТ) ---
    if (!context.Categories.Any(c => c.Name == "Футболки"))
    {
        var shirtCat = new Category { Name = "Футболки" };
        context.Categories.Add(shirtCat);
        context.SaveChanges();

        var shirtProd = new Product
        {
            Name = "Футболка Goose-Style",
            Description = "Оверсайз футболка для зумерів. Якісна бавовна, крутий гусак.",
            BasePrice = 550,
            CategoryId = shirtCat.Id,
            ImageUrl = "images/tshirt.jpg" // Переконайся, що файл є у wwwroot/images/
        };
        context.Products.Add(shirtProd);
        context.SaveChanges();

        // Додаємо варіанти як РОЗМІРИ, а не мілілітри
        context.ProductVariants.AddRange(
            new ProductVariant { ProductId = shirtProd.Id, Size = "S", Color = "black", Price = 550, Title = "Black S", StockQuantity = 5 },
            new ProductVariant { ProductId = shirtProd.Id, Size = "M", Color = "black", Price = 550, Title = "Black M", StockQuantity = 10 },
            new ProductVariant { ProductId = shirtProd.Id, Size = "L", Color = "black", Price = 580, Title = "Black L (XL style)", StockQuantity = 5 },
            new ProductVariant { ProductId = shirtProd.Id, Size = "M", Color = "white", Price = 550, Title = "White M", StockQuantity = 7 }
        );
        context.SaveChanges();
    }
}

// 6. МАРШРУТИЗАЦІЯ BLAZOR
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();