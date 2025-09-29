using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace TripSync.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }
        public string ErrorMessage { get; set; }

        private readonly IConfiguration _config;

        public LoginModel(IConfiguration config)
        {
            _config = config;
        }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            string connectionString = _config.GetConnectionString("TripSyncDb");
            string hashedPassword = HashPassword(Password);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                //Check credentials
                string query = "SELECT user_id FROM users WHERE email = @Email AND password_hash = @Password";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);

                    object result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        ErrorMessage = "Invalid email or password.";
                        return Page();
                    }

                    //Store user_id in session
                    HttpContext.Session.SetInt32("UserId", Convert.ToInt32(result));
                }
            }

            //Redirect to home after login
            return RedirectToPage("/Index");
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
