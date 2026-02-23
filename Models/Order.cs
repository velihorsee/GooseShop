using System.ComponentModel.DataAnnotations;

namespace GooseShop.Models
{
    public class Order
    {
        public int Id { get; set; }

        // Зв'язок з користувачем (автореєстрація)
        public int? UserId { get; set; }
        public User? User { get; set; }

        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = "";

        [EmailAddress(ErrorMessage = "Некоректний формат Email")]
        public string Email { get; set; } = "";

        // Синхронізуємо назву з Checkout.razor (там використовували CreatedAt)
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();

        // Замість bool IsCompleted краще використовувати string Status
        // Це дозволить мати статуси: "Нове", "Оплачено", "Відправлено", "Скасовано"
        public string Status { get; set; } = "Нове";

        public string ShippingAddress { get; set; } = "";
    }
}