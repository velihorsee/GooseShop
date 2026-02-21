using System.ComponentModel.DataAnnotations;

namespace GooseShop.Models
{
    public class ProductConstructor
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Required]
        public string ModelPath { get; set; } = string.Empty; // Шлях до .glb файлу

        public ConstructorType Type { get; set; }
    }

    public enum ConstructorType
    {
        Cup,
        TShirt
    }
}