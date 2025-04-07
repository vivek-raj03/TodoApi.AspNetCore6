using MySql.Data.MySqlClient;
using UserAPI.Models;

namespace TODO_API_net.Models
{
    public class DBContext
    {
        public string ConnectionString { get; set; }
        public DBContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public UserResponse GetAuthUser(string username, string password)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string query = @"SELECT * FROM users WHERE Username = @Username; ";

                    using (MySqlCommand sql = new MySqlCommand(query, conn))
                    {
                        sql.Parameters.AddWithValue("@Username", username);

                        using (MySqlDataReader reader = sql.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["PasswordHash"].ToString();
                                


                                if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                                {
                                    return new UserResponse
                                    {
                                        Id = Convert.ToInt32(reader["Id"]),
                                        Username = reader["Username"].ToString(),
                                        FirstName = reader["FirstName"].ToString(),
                                        LastName = reader["LastName"].ToString(),
                                        MiddleName = reader["MiddleName"].ToString(),
                                        Email = reader["Email"].ToString()


                                    };
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error logging in user: {ex.Message}");
                }
            }

            return null;
        }

    }
}
