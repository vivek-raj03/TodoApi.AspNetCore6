using konnect_player_info.Model.Manager;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using TODO_API_net.Models.Generic;

namespace TODO_API_net.Models.Manager
{
    public class TodoManager : BaseManager
    {

        public List<Dictionary<string, object>> Get_Todos(int id)
        {
            var todos = new List<Dictionary<string, object>>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string query = @"
                SELECT users.Username, 
                       todos.Id,     
                       todos.Title, 
                       todos.Description, 
                       todos.IsCompleted 
                FROM users 
                JOIN todos ON users.Id = todos.UserId 
                WHERE users.Id = @Id and todos.Status = 'A' ";

                    MySqlCommand sql = new MySqlCommand(query, conn);
                    sql.Parameters.AddWithValue("@Id", id);
                    MySqlDataReader reader = sql.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    todos = dt.AsEnumerable()
                        .Select(row => dt.Columns.Cast<DataColumn>()
                            .ToDictionary(column => column.ColumnName, column => row[column] == DBNull.Value ? null : row[column]))
                        .ToList();

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching todos: {ex.Message}");
                    return null;
                }
            }

            return todos;
        }

        public Todo CreateTodo(Todo todo)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string query = @"
                INSERT INTO todos (UserId, Title, Description) 
                VALUES (@UserId, @Title, @Description);
                SELECT LAST_INSERT_ID();";

                    using (MySqlCommand sql = new MySqlCommand(query, conn))
                    {
                        sql.Parameters.AddWithValue("@UserId", todo.UserId);
                        sql.Parameters.AddWithValue("@Title", todo.Title);
                        sql.Parameters.AddWithValue("@Description", todo.Description);

                        object result = sql.ExecuteScalar();
                        if (result != null)
                        {
                            todo.Id = Convert.ToInt32(result);
                            todo.Status = "A";
                            todo.IsCompleted = false;
                        }
                        else
                        {
                            throw new Exception("Failed to retrieve inserted ID.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating todo: {ex.Message}");
                    return null;
                }
            }
            return todo;
        }

        public Todo GetTodoById(int id, string status = "")
        {
            var todo = new Todo();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string query = @"
                SELECT * FROM Todos WHERE Id = @Id";
                    if (status == "A") query += " AND Status = 'A'";
                    else if (status == "D") query += " AND Status = 'D'";

                    using (MySqlCommand sql = new MySqlCommand(query, conn))
                    {
                        sql.Parameters.AddWithValue("@Id", id);
                        using (MySqlDataReader reader = sql.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                todo.Id = reader.GetInt32("Id");
                                todo.UserId = reader.GetInt32("UserId");
                                todo.Title = reader.GetString("Title");
                                todo.Description = reader.GetString("Description");
                                todo.IsCompleted = reader.GetBoolean("IsCompleted");
                                todo.Status = reader.GetString("Status");
                            }
                            else
                            {
                                return null; 
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching todo: {ex.Message}");
                    return null;
                }
            }
            return todo;
        }

        public String TodoStatus(int Id, string Status) // delete or not
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string query = @"
               UPDATE Todos set Status = @Status WHERE Id = @Id
";
                    using (MySqlCommand sql = new MySqlCommand(query, conn))
                    {
                        sql.Parameters.AddWithValue("@Id", Id);
                        sql.Parameters.AddWithValue("@Status", Status);

                        sql.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting todo: {ex.Message}");
                    return "Error Todo Status Update ";
                }
            }
            return "Todo Status Updated successfully";
        }

        public Todo IsComplete(Boolean IsCompleted, int Id) // null if no todo
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string query = @"
               UPDATE Todos set IsCompleted = @IsCompleted WHERE Id = @Id
";
                    using (MySqlCommand sql = new MySqlCommand(query, conn))
                    {
                        sql.Parameters.AddWithValue("@Id", Id);
                        sql.Parameters.AddWithValue("@IsCompleted", IsCompleted);
                        sql.ExecuteNonQuery();
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting todo: {ex.Message}");
                    return null;
                }
            }
            return this.GetTodoById(Id);

        }
    
        public Todo UpdateTodoDetails(Todo Todo)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string query = @" UPDATE Todos set Title = @Title, Description =@Description, Status = @Status,IsCompleted = @IsCompleted WHERE Id = @Id";
                    using(MySqlCommand sql = new MySqlCommand(query, conn))
                    {
                        sql.Parameters.AddWithValue("@Id", Todo.Id);
                        sql.Parameters.AddWithValue("@Title", Todo.Title);
                        sql.Parameters.AddWithValue("@Description", Todo.Description);
                        sql.Parameters.AddWithValue("@Status", Todo.Status);
                        sql.Parameters.AddWithValue("@IsCompleted", Todo.IsCompleted);
                        sql.ExecuteNonQuery();
                        return this.GetTodoById(Todo.Id);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Updating todo: {ex.Message}");
                    return null;
                }
            }
        }

    }
}
