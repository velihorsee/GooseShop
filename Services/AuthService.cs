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

        public User? CurrentUser { get => _currentUser; private set => _currentUser = value; }
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

        public async Task<bool> Register(string fullName, string email, string password)
        {
            using var context = _contextFactory.CreateDbContext();
            if (await context.Users.AnyAsync(u => u.Email == email)) return false;

            var newUser = new User
            {
                FullName = fullName,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Phone = ""
            };

            context.Users.Add(newUser);
            await context.SaveChangesAsync();
            CurrentUser = newUser;
            NotifyStateChanged();
            return true;
        }

        public void Logout() { CurrentUser = null; NotifyStateChanged(); }
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}