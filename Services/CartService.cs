using GooseShop.Models;

namespace GooseShop.Services;

public class CartService
{
    public List<CartItem> Items { get; private set; } = new();
    public event Action? OnChange;

    // Оновлений метод: тепер він приймає або ціну, або бере її з продукту
    public void AddToCart(Product product, string options, decimal? price = null, int quantity = 1, string? imageUrl = null)
    {
        // Якщо ціна не передана (наприклад, з каталогу), беремо базову
        decimal finalPrice = price ?? product.BasePrice;

        Items.Add(new CartItem
        {
            Product = product,
            Options = options,              // Наші "330мл, Біла"
            Price = finalPrice,             // Ціна, яку ми вирахували на сторінці
            UserUploadedImageUrl = imageUrl, // Якщо це з конструктора
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

    public void RemoveFromCart(int productId)
    {
        var item = Items.FirstOrDefault(i => i.Product.Id == productId);
        if (item != null)
        {
            Items.Remove(item);
            NotifyStateChanged();
        }
    }
    // Метод для старих компонентів (щоб прибрати помилку)
    public decimal GetTotal()
    {
        return Items.Sum(i => i.Price * i.Quantity);
    }

    // Тепер сума рахується по ціні з CartItem, а не з Product!
    public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);

    public void NotifyStateChanged() => OnChange?.Invoke();
}