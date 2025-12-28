namespace WardSystem.Services
{
    public class AuthService
    {
        public bool IsAuthenticated { get; private set; } = false;

        public bool Login(string password)
        {
            if (password == "idegseb2025")
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