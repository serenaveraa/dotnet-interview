using System.Text.Json.Serialization;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public long TodoListId { get; set; }
        public string Description { get; set; } = null!;
        public bool Completed { get; set; } = false;

        [JsonIgnore]
        public TodoList TodoList { get; set; } = null!;
    }
}
