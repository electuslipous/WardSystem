using Microsoft.Extensions.Configuration;

namespace WardSystem.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }
        public bool IsAuthenticated { get; private set; } = false;

        public bool Login(string password)
        {
            string? correctPassword = _config["Auth:Password"];
            if (!string.IsNullOrEmpty(correctPassword) && password == correctPassword)
            {
                IsAuthenticated = true;
                return true;
            }
            return false;
        }

        public void Logout()
        {
            IsAuthenticated = false;
        }
    }
}