using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using TODO_API_net.Models.Generic;
using TODO_API_net.Models.Manager;
using UserAPI.Models;

namespace TODO_API_net.Controllers
{
    [Route("Todo/")]
    [ApiController]
    public class TodoController : ControllerBase
    {

        [HttpGet("Todos/{UserId}")]
        public IActionResult GetPlayerTODOS(int UserId)
        {
            var Todo = new TodoManager();
            var TODOS = Todo.Get_Todos(UserId);
            if (TODOS == null)
            {
                return NotFound(new { message = "User Todos not found." });
            }
            return Ok(TODOS);
        }

        [HttpGet("GetTodo/{Id}")]
        public IActionResult GetTodoById(int Id)
        {
            var task = new TodoManager();
            var todo = task.GetTodoById(Id);
            if (todo == null)
            {
                return NotFound(new { message = "Todo not found." });
            }
            return Ok(todo);
        }

        [HttpPost("NewTodo")]
        public IActionResult NewTodo([FromBody] Todo todo)
        {
            var manager = new UserManager();
            var user = manager.GetUserById(todo.UserId);
            if (user.Id == 0)
            {
                return NotFound(new { message = "User not found." });
            }

            var task = new TodoManager();
            var new_todo = task.CreateTodo(todo);
            return Ok(new { message = "Todo Created Successfully" ,  data = new_todo });

        }

        [HttpDelete("DeleteTodo/{Id}")] 
        public IActionResult DeleteTodo(int Id)
        {
            var task = new TodoManager();
            var isExist = task.GetTodoById(Id, "A");
            if (isExist == null)
            {
                return NotFound(new { message = "Todo not found." });
            }

            var new_todo = task.TodoStatus(Id, "D");
            return Ok(new { message = new_todo });

        }

        [HttpPut("RetriveTodo/{Id}")] 
        public IActionResult RetriveTodo(int Id)
        {
            var task = new TodoManager();
            var isExist = task.GetTodoById(Id, "D");
            if (isExist == null)
            {
                return NotFound(new { message = "Todo not found." });
            }

            var new_todo = task.TodoStatus(Id, "A");
            return Ok(new { message = new_todo });

        }


        [HttpPut("MarkAsComplete/{Id}/Status/{Status}")]
        public IActionResult UpdateTodoIsComplete(int Id, Boolean Status)
        {
            var task = new TodoManager();
            var todo = task.GetTodoById(Id);
            if (todo == null)
            {
                return NotFound(new { message = "Todo not found." });
            }
            var UpdateData = task.IsComplete(Status, Id);
            if (UpdateData == null)
            {
                return NotFound(new { message = "Error updating todo." });
            }
            return Ok(new { message = "Todo Updated Successfully", data = UpdateData });
        }

        [HttpPut("TodoUpdateDetails")]
        public IActionResult UpdateTodoDetails(Todo todo)
        {
            var task = new TodoManager();
            var isExist = task.GetTodoById(todo.Id);
            if (isExist == null)
            {
                return NotFound(new { message = "Todo not found." });
            }
            var new_todo = task.UpdateTodoDetails(todo);
            return Ok(new { message = "Todo Updated Successfully", data = new_todo });
        }




    }
}
