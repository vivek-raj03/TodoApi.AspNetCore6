using MySql.Data.MySqlClient;
using UserAPI.Models;

namespace TODO_API_net.Models.basicAuth
{
    public class BasicAuthServices
    {
        public static UserResponse _listusers;
        public static string conn;

        public interface IBasicAuthService
        {
            Task<UserResponse> Authenticate(string username, string password);
        }


        public class BasicAuthService : IBasicAuthService
        {
            public async Task<UserResponse> Authenticate(string username, string password)
            {
                var db = new DBContext(conn);
                var user = await Task.Run(() => db.GetAuthUser(username, password));

                if (user == null)
                {
                    return null;
                }
                return user;
            }
        }
    }
}
