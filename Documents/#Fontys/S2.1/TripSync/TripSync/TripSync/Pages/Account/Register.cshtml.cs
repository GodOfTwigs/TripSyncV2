using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace TripSync.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }
        [BindProperty] public string ConfirmPassword { get; set; }

        public string ErrorMessage { get; set; }
        private readonly IConfiguration _config;

        public RegisterModel(IConfiguration config)
        {
            _config = config;
        }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            Debug.WriteLine($"DEBUG: Name={Name}, Email={Email}, Password={Password}");

            // Input validation
            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = "Name is required.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email is required.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Password is required.";
                return Page();
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return Page();
            }

            string connectionString = _config.GetConnectionString("TripSyncDb");
            string hashedPassword = HashPassword(Password);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check if email already exists
                string checkUser = "SELECT COUNT(*) FROM users WHERE email = @Email";
                using (SqlCommand cmd = new SqlCommand(checkUser, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", Email.Trim());
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                    {
                        ErrorMessage = "This email is already registered.";
                        return Page();
                    }
                }

                // Insert new user safely
                string insertQuery = @"INSERT INTO users (name, email, password_hash) 
                                       VALUES (@Name, @Email, @Password)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", Name.Trim());
                    cmd.Parameters.AddWithValue("@Email", Email.Trim());
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);

                    cmd.ExecuteNonQuery();
                }
            }

            // Redirect to login after successful registration
            return RedirectToPage("/Account/Login");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
