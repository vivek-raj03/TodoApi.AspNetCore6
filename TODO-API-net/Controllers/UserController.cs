using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Models;

[Route("User/")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var manager = new UserManager();
        var users = manager.GetAllUsers();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public IActionResult GetPlayerById(int id)
    {
        var manager = new UserManager();
        var user = manager.GetUserById(id);
        if (user.Id == 0)
        {
            return NotFound(new { message = "User not found." });
        }
        return Ok(user);
    }

    [HttpPut("UserUpdate")]
    public IActionResult UpdateUser([FromBody] User user)
    {
        var manager = new UserManager();

        if (user?.Id == null || user.Id <= 0)
            return BadRequest("Id is missing or invalid");

        var data = manager.GetUserById(user.Id);
        if (data.Id == 0)
        {
            return NotFound(new { message = "User not found." });
        }

        //data.Username = user.Username;
        //data.Email = user.Email;

        var isSuccess = manager.UpdateUser(user);

        if (!isSuccess) return StatusCode(500, "Failed to update user data!");

        return Ok(new { message = "User data updated successfully." });
    }

    [HttpDelete("UserDelete/{id}")]
    public IActionResult DeleteUser(int Id)
    {
        var manager = new UserManager();


        var data = manager.GetUserById(Id);
        if (data.Id == 0)
        {
            return NotFound(new { message = "User not found." });
        }

        var isSuccess = manager.DeleteUser(data.Id);
        if (!isSuccess) return StatusCode(500, "Failed to Delete user data!");

        return Ok(new { message = "User data Deleted successfully." });
    }

    [HttpDelete("UserRetrive/{id}")]
    public IActionResult RetriveUser(int Id)
    {
        var manager = new UserManager();


        var data = manager.GetUserById(Id, "D");
        Console.WriteLine(data);
        if (data.Id == 0)
        {
            return NotFound(new { message = "User not found." });
        }

        var isSuccess = manager.DeleteUser(data.Id, false);
        if (!isSuccess) return StatusCode(500, "Failed to Delete user data!");

        return Ok(new { message = "User data Deleted successfully." });
    }
    [AllowAnonymous]
    [HttpPost("NewUser")] // register
    public IActionResult NewUser([FromBody] User user)
    {
        var manager = new UserManager();
        var duplicateMessage = manager.IsUNIQUE(user);
        if (duplicateMessage != null)
        {
            return BadRequest(new { message = duplicateMessage });
        }

        manager.NewUser(user);

        return Ok(new { message = "User Created Successfully" });
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login([FromBody] UserLogin UserLogin)
    {
        var manager = new UserManager();
        var isok = manager.GetUserByUsername(UserLogin.Username);
        if(isok == null)
        {
            return NotFound(new { message = "User not Found!" });
        }

        var data = manager.Login(UserLogin);
        if (data == null)
        {
            return NotFound(new { message = "Invalid Credentials! " });
        }
        return Ok(data);
    }

    [HttpPut("changepassword")]
    public IActionResult ChangePassword([FromBody] ChangePass ChangePass)
    {
        var manager = new UserManager();
        var mess = manager.ChangePassword(ChangePass);

        return Ok(new { message = mess });
    }

}
