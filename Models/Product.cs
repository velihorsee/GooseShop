using System.ComponentModel.DataAnnotations;

namespace GooseShop.Models
{
    // 2. Базовий товар
    public partial class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty; // Шлях до фото "пустого" товару

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
