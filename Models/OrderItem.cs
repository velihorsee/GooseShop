using System.ComponentModel.DataAnnotations;
namespace GooseShop.Models
{
    // 4. Елемент замовлення (тут зберігається результат конструктора)
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        // Перевірте, чи є це поле (помилка на скріншоті 3 може бути через це)
        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        // ПЕРЕВІРТЕ ЦІ НАЗВИ:
        public decimal PriceAtPurchase { get; set; }
        public string CustomConfigurationJson { get; set; } = string.Empty;
        public string? UserUploadedImageUrl { get; set; }
    }
}
