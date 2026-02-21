using GooseShop.Data;
using GooseShop.Models;
using Microsoft.EntityFrameworkCore;
using GooseShop.Services;
namespace GooseShop.Services;

public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    // Отримати всі товари (для каталогу)
    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .AsNoTracking() // Це вимикає відстеження змін, що значно прискорює запит
            .ToListAsync();
    }

    // Отримати товар за ID (для сторінки конструктора)
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}