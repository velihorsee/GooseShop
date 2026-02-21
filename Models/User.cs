using System.ComponentModel.DataAnnotations;

namespace GooseShop.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введіть повне ім'я")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        // Додаємо це поле, бо сервіс намагається його оновити
        public string Phone { get; set; } = string.Empty;

        public string Role { get; set; } = "Customer";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}