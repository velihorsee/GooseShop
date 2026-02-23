namespace GooseShop.Models;

public class CartItem
{
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; } = 1;
    // Сюди ми будемо записувати JSON з ProductDetails
    public string CustomConfigurationJson { get; set; } = string.Empty;
    public string? UserUploadedImageUrl { get; set; } // Нове поле
    public decimal TotalPrice => Product.BasePrice * Quantity;
    public string? Options { get; set; } // Додай це!
    public decimal Price { get; set; }   // Додай це!
}