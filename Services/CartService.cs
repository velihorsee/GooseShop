using GooseShop.Models;

namespace GooseShop.Services;

public class CartService
{
    public List<CartItem> Items { get; private set; } = new();
    public event Action? OnChange;

    public void AddToCart(Product product, string jsonConfig, string? imageUrl, int quantity)
    {
        Items.Add(new CartItem
        {
            Product = product,
            CustomConfigurationJson = jsonConfig,
            UserUploadedImageUrl = imageUrl,
            Quantity = quantity
        });
        NotifyStateChanged();
    }

    public void RemoveItem(CartItem item)
    {
        Items.Remove(item);
        NotifyStateChanged();
    }

    public void ClearCart()
    {
        Items.Clear();
        NotifyStateChanged();
    }

    // Для CartSidebar.razor
    public void RemoveFromCart(int productId)
    {
        var item = Items.FirstOrDefault(i => i.Product.Id == productId);
        if (item != null) { Items.Remove(item); NotifyStateChanged(); }
    }

    public decimal TotalPrice => GetTotal();
    public decimal GetTotal() => Items.Sum(i => i.Product.BasePrice * i.Quantity);
    public void NotifyStateChanged() => OnChange?.Invoke();
}

    
