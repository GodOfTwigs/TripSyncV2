using DataAccess;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public (bool Success, int? UserId, string ErrorMessage) Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, null, "Username and password are required.");

            string passwordhash = HashPassword(password);

            int? userId = _userRepository.ValidateUser(username, passwordhash);
            if (userId.HasValue)
                return (true, userId.Value, null);

            return (false, null, "Invalid username or password.");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public (bool Success, string Error) Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Username and password are required.");

            if (_userRepository.UsernameExists(username))
                return (false, "Username already exists.");

            string passwordHash = HashPassword(password);
            _userRepository.AddUser(username, passwordHash);

            return (true, null);
        }
    }
}

