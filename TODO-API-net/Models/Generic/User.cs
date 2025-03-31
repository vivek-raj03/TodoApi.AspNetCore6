namespace UserAPI.Models
{
    public class User
    {
        public int Id { get; set; } 
        public string? Username { get; set; }
        public string? PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string? Status { get; set; } = string.Empty;

       
    }

    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }   
    public class ChangePass
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

    }

    public class UserResponse
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
    }



}
