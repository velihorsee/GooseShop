using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GooseShop.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        [Required]
        public string Title { get; set; } = string.Empty; // Наприклад: "330мл, Біла"

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } // Кінцева ціна для цієї варіації

        public int StockQuantity { get; set; } // Скільки таких гусаків на складі

        // Поля для фільтрації
        public string? Color { get; set; } // "white", "black", "red"
        public string? Size { get; set; }  // "330", "440" для чашок або "S", "M" для футболок

        public string? ImageUrl { get; set; } // Можна додати окреме фото для кожного кольору
    }
}