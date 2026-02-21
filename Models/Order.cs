using System.ComponentModel.DataAnnotations;
namespace GooseShop.Models
{
    // 3. Замовлення
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = "";
        [EmailAddress(ErrorMessage = "Некоректний формат Email")]
        public string Email { get; set; } = "";
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
        public bool IsCompleted { get; set; } = false;
        public string ShippingAddress { get; set; } = "";
        
    }
}
