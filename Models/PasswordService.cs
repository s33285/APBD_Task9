namespace User_Panel.Models
{
    public class PasswordService
    {
        private readonly string _pepper;

        public PasswordService(IConfiguration config)
        {
            _pepper = config["Security:Pepper"]
                ?? throw new InvalidOperationException("Missing pepper configuration (Security:Pepper).");
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password + _pepper, workFactor: 12);
        }

        public bool Verify(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password + _pepper, storedHash);
        }
    }
}
