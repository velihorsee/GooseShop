using System.ComponentModel.DataAnnotations;

namespace GooseShop.Models
{
    public partial class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal BasePrice { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // --- ДОДАЙ ЦЕЙ РЯДОК ---
        // Це дозволить завантажувати всі варіанти товару через .Include(p => p.Variants)
        public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}