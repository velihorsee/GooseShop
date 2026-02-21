namespace GooseShop.Services {  // <--- Перевірте цей рядок
public class AdminService
{
    public event Action? OnChange;
    private bool _isAuthenticated;

    public bool IsAuthenticated
    {
        get => _isAuthenticated;
        private set { _isAuthenticated = value; OnChange?.Invoke(); }
    }

    public string AdminPassword { get; } = "12345";

    public bool Login(string password)
    {
        if (password == AdminPassword)
        {
            IsAuthenticated = true;
            return true;
        }
        return false;
    }

    public void Logout() => IsAuthenticated = false;
}
}