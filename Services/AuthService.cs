using GooseShop.Data;
using GooseShop.Models;
using Microsoft.EntityFrameworkCore;

namespace GooseShop.Services
{
    public class AuthService
    {
        // Поле для фабрики контексту (вирішує помилку _contextFactory)
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        // Поле для поточного користувача (вирішує помилку _currentUser)
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

        public async Task<bool> Login(string email, string password)
        {
            using var context = _contextFactory.CreateDbContext();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                CurrentUser = user;
                NotifyStateChanged();
                return true;
            }
            return false;
        }

        public void Logout()
        {
            CurrentUser = null;
            NotifyStateChanged();
        }

        // Вирішує помилки Name, Phone та tempPassword
        public async Task<bool> UpdateUserAsync(User updatedUser, string? newPassword = null)
        {
            using var context = _contextFactory.CreateDbContext();
            var user = await context.Users.FindAsync(updatedUser.Id);

            if (user == null) return false;

            // Використовуємо FullName замість Name, як у вашій моделі
            user.FullName = updatedUser.FullName;
            user.Phone = updatedUser.Phone;
            user.Email = updatedUser.Email;

            if (!string.IsNullOrEmpty(newPassword))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }

            await context.SaveChangesAsync();
            CurrentUser = user;
            NotifyStateChanged();
            return true;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}