using GooseShop.Data;
using GooseShop.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace GooseShop.Services
{
    public class AuthService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private User? _currentUser;

        public User? CurrentUser
        {
            get => _currentUser;
            private set => _currentUser = value;
        }

        public bool IsLoggedIn => CurrentUser != null;
        public event Action? OnChange;

        public AuthService(IDbContextFactory<AppDbContext> dbFactory)
        {
            _contextFactory = dbFactory;
        }

        // --- ВХІД ---
        public async Task<bool> Login(string email, string password)
        {
            using var context = _contextFactory.CreateDbContext();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

            // Перевірка хешу пароля
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                CurrentUser = user;
                NotifyStateChanged();
                return true;
            }
            return false;
        }

        // --- РЕЄСТРАЦІЯ (НОВИЙ МЕТОД) ---
        public async Task<bool> Register(string fullName, string email, string password)
        {
            using var context = _contextFactory.CreateDbContext();

            // Перевіряємо, чи пошта вже зайнята
            bool exists = await context.Users.AnyAsync(u => u.Email == email);
            if (exists) return false;

            var newUser = new User
            {
                FullName = fullName,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password), // Хешуємо пароль перед збереженням
                Phone = "" // Початкове значення, щоб не було null
            };

            context.Users.Add(newUser);
            await context.SaveChangesAsync();

            // Автоматичний вхід після реєстрації
            CurrentUser = newUser;
            NotifyStateChanged();
            return true;
        }

        // --- ВИХІД ---
        public void Logout()
        {
            CurrentUser = null;
            NotifyStateChanged();
        }

        // --- ОНОВЛЕННЯ ПРОФІЛЮ ---
        public async Task<bool> UpdateUserAsync(User updatedUser, string? newPassword = null)
        {
            using var context = _contextFactory.CreateDbContext();
            var user = await context.Users.FindAsync(updatedUser.Id);

            if (user == null) return false;

            user.FullName = updatedUser.FullName;
            user.Phone = updatedUser.Phone;
            user.Email = updatedUser.Email;

            if (!string.IsNullOrEmpty(newPassword))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }

            await context.SaveChangesAsync();

            // Оновлюємо поточного користувача в пам'яті
            CurrentUser = user;
            NotifyStateChanged();
            return true;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}