using System.Data.SqlClient;

namespace DataAccess
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Returns user ID if credentials are valid, otherwise null
        public int? ValidateUser(string username, string password)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = @"
                SELECT user_id
                FROM Users
                WHERE name = @name AND password_hash = @password_hash";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", username);
            cmd.Parameters.AddWithValue("@password_hash", password);

            var result = cmd.ExecuteScalar();
            if (result != null && int.TryParse(result.ToString(), out int userId))
            {
                return userId;
            }

            return null;
        }

        // Check if username already exists
        public bool UsernameExists(string username)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var query = "SELECT COUNT(*) FROM Users WHERE name = @name";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", username);
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        // Insert a new user
        public void AddUser(string username, string passwordHash)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var query = "INSERT INTO Users (name, password_hash) VALUES (@name, @password_hash)";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", username);
            cmd.Parameters.AddWithValue("@password_hash", passwordHash);
            cmd.ExecuteNonQuery();
        }
    }
}
