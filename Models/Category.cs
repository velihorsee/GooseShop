using System.ComponentModel.DataAnnotations;

namespace GooseShop.Models
{
    // 1. Категорії (Чашки, Футболки)
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = new();
    }
}
