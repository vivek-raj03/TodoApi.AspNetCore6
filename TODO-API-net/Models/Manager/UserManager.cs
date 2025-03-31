using konnect_player_info.Model.Manager;
using MySql.Data.MySqlClient;
using System.Data;

namespace UserAPI.Models
{
    public class UserManager : BaseManager
    {
        public List<User> GetAllUsers()
        {
            var users = new List<User>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string cmd = "SELECT * FROM Users WHERE Status = 'A' ";
                    MySqlCommand sql = new MySqlCommand(cmd, conn);
                    MySqlDataReader reader = sql.ExecuteReader();

                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32("Id"),
                            Username = reader.GetString("Username"),
                            FirstName = reader.GetString("FirstName"),
                            LastName = reader.GetString("LastName"),
                            MiddleName = reader.GetString("MiddleName"),
                            Email = reader.GetString("Email"),
                            PasswordHash = reader.GetString("PasswordHash"),
                            Status = reader.GetString("Status")

                        });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching users: {ex.Message}");
                }
            }
            return users;
        }

        public User GetUserById(int id, string Status = "A")
        {
            User user = new User();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string cmd = "SELECT * FROM Users WHERE Id = @Id and Status = @Status ";
                    MySqlCommand sql = new MySqlCommand(cmd, conn);
                    sql.Parameters.AddWithValue("@Id", id);
                    sql.Parameters.AddWithValue("@Status", Status);

                    MySqlDataReader reader = sql.ExecuteReader();

                    if (reader.HasRows)
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        user = new User
                        {
                            Id = int.Parse(dt.Rows[0]["Id"].ToString()),
                            Username = dt.Rows[0]["Username"].ToString(),
                            FirstName = dt.Rows[0]["FirstName"].ToString(),
                            LastName = dt.Rows[0]["LastName"].ToString().ToString(),
                            MiddleName = dt.Rows[0]["MiddleName"].ToString().ToString(),
                            Email = dt.Rows[0]["Email"].ToString(),
                            PasswordHash = dt.Rows[0]["PasswordHash"].ToString(),
                            Status = dt.Rows[0]["Status"].ToString(),
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching user: {ex.Message}");
                }
            }
            return user;
        }
        public User GetUserByUsername(string Username, string Status = "A")
        {
            User user = new User();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string cmd = "SELECT * FROM Users WHERE Username = @Username and Status = @Status; ";
                    MySqlCommand sql = new MySqlCommand(cmd, conn);
                    sql.Parameters.AddWithValue("@Username", Username);
                    sql.Parameters.AddWithValue("@Status", Status);

                    MySqlDataReader reader = sql.ExecuteReader();

                    if (reader.HasRows)
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        user = new User
                        {
                            Id = int.Parse(dt.Rows[0]["Id"].ToString()),
                            Username = dt.Rows[0]["Username"].ToString(),
                            FirstName = dt.Rows[0]["FirstName"].ToString(),
                            LastName = dt.Rows[0]["LastName"].ToString().ToString(),
                            MiddleName = dt.Rows[0]["MiddleName"].ToString().ToString(),
                            Email = dt.Rows[0]["Email"].ToString(),
                            PasswordHash = dt.Rows[0]["PasswordHash"].ToString(),
                            Status = dt.Rows[0]["Status"].ToString(),

                        };
                        return user;
                    }
                    reader.Close();
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching user: {ex.Message}");
                }
            }
            return null;
        }
        public string? IsUNIQUE(User user)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string cmd = "SELECT Username, Email FROM Users WHERE Username = @Username OR Email = @Email";
                    MySqlCommand sql = new MySqlCommand(cmd, conn);
                    sql.Parameters.AddWithValue("@Username", user.Username);
                    sql.Parameters.AddWithValue("@Email", user.Email);

                    using (MySqlDataReader reader = sql.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (reader["Username"].ToString() == user.Username)
                                {
                                    return "Username already exists.";
                                }
                                if (reader["Email"].ToString() == user.Email)
                                {
                                    return "Email already exists.";
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking uniqueness: {ex.Message}");
                }
            }
            return null;
        }

        public bool UpdateUser(User user)
        {
            bool isUpdated = false;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string cmd = @"
                UPDATE users 
                SET  
                    Username = @Username,
                    FirstName = @FirstName, 
                    LastName = @LastName,
                    MiddleName = @MiddleName,
                    Email = @Email,
                    PasswordHash = @PasswordHash,
                    Status = @Status
                WHERE Id = @Id;
            ";

                    MySqlCommand sql = new MySqlCommand(cmd, conn);
                    sql.Parameters.AddWithValue("@Id", user.Id);
                    sql.Parameters.AddWithValue("@Username", user.Username);
                    sql.Parameters.AddWithValue("@FirstName", user.FirstName);
                    sql.Parameters.AddWithValue("@LastName", user.LastName);
                    sql.Parameters.AddWithValue("@MiddleName", user.MiddleName ?? null);
                    sql.Parameters.AddWithValue("@Email", user.Email);
                    sql.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    sql.Parameters.AddWithValue("@Status", user.Status);

                    int rowsAffected = sql.ExecuteNonQuery();
                    isUpdated = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating user: {ex.Message}");
                }
            }

            return isUpdated;
        }

        public bool DeleteUser(int Id, bool IsDelete = true)
        {
            var IsOkay = false;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string cmd = @"
                        UPDATE users 
                        SET  
                        ";

                    if (IsDelete) cmd += " Status = 'D' ";
                    else cmd += " Status = 'A' ";
                    MySqlCommand sql = new MySqlCommand(cmd, conn);
                    sql.Parameters.AddWithValue("@Id", Id);
                    sql.ExecuteNonQuery();
                    conn.Close();
                    IsOkay = true;

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Updating user: {ex.Message}");
                }
            }
            return IsOkay;
        }

        public UserResponse Login(UserLogin UserLogin)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string query = @"SELECT * FROM users WHERE Username = @Username; ";

                    using (MySqlCommand sql = new MySqlCommand(query, conn))
                    {
                        sql.Parameters.AddWithValue("@Username", UserLogin.Username);

                        using (MySqlDataReader reader = sql.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["PasswordHash"].ToString();
                                var isExisiting = this.GetUserById(Convert.ToInt32(reader["Id"]));
                                if(isExisiting.Id == 0)
                                {
                                    return null;
                                }


                                if (BCrypt.Net.BCrypt.Verify(UserLogin.Password, storedHash))
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

        public bool NewUser(User user)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    // Hash the password before storing
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                    string cmd = @"
                INSERT INTO users (Username, FirstName, LastName, MiddleName, Email, PasswordHash, Status) 
                VALUES (@Username, @FirstName, @LastName, @MiddleName, @Email, @PasswordHash, 'A');
            ";

                    using (MySqlCommand sql = new MySqlCommand(cmd, conn))
                    {
                        sql.Parameters.AddWithValue("@Username", user.Username);
                        sql.Parameters.AddWithValue("@FirstName", user.FirstName);
                        sql.Parameters.AddWithValue("@LastName", user.LastName);
                        sql.Parameters.AddWithValue("@MiddleName", string.IsNullOrEmpty(user.MiddleName) ? DBNull.Value : user.MiddleName);
                        sql.Parameters.AddWithValue("@Email", user.Email);
                        sql.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                        int rowsAffected = sql.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting user: {ex.Message}");
                    return false;
                }
            }
        }

        public string ChangePassword(ChangePass ChangePass)
        {

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string query = @"SELECT ID, PasswordHash FROM users WHERE Username = @Username;";
                    using (MySqlCommand sql = new MySqlCommand(query, conn))
                    {
                        sql.Parameters.AddWithValue("@Username", ChangePass.Username);
                        using (MySqlDataReader reader = sql.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["PasswordHash"].ToString();

                                if (BCrypt.Net.BCrypt.Verify(ChangePass.OldPassword, storedHash))
                                {
                                    reader.Close(); 

                                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(ChangePass.NewPassword);
                                    string cmd = @"UPDATE users SET PasswordHash = @PasswordHash WHERE Username = @Username;";

                                    using (MySqlCommand sql2 = new MySqlCommand(cmd, conn))
                                    {
                                        sql2.Parameters.AddWithValue("@Username", ChangePass.Username);
                                        sql2.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                                        int rowsAffected = sql2.ExecuteNonQuery();
                                        if (rowsAffected > 0)
                                            return "Password Changed Successfully";
                                        else
                                            return "Failed to update password!";
                                    }
                                }
                                else
                                {
                                    return "Old Password is incorrect";
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error changing password: {ex.Message}");
                    return "An error occurred while changing the password.";
                }
            }

            return "User not found!";
        }

    }
}
