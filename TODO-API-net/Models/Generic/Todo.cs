namespace TODO_API_net.Models.Generic
{
    public class Todo
    {
        public int Id { get; set; } 
        public int UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; }
        public string? Status { get; set; } = string.Empty;
    }
   
}
