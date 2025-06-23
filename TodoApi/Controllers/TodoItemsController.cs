using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/todolists/{listId}")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> CreateItem(long listId, CreateTodoItem payload)
        {
            var todoList = await _context.TodoList.FindAsync(listId);
            if (todoList == null)
                return NotFound($"List {listId} not found.");

            var item = new TodoItem
            {
                TodoListId = listId,
                Description = payload.Description,
                Completed = false
            };

            _context.TodoItem.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItem), new { listId = listId, itemId = item.Id }, item);
        }

        [HttpGet("{itemId}")]
        public async Task<ActionResult<TodoItem>> GetItem(long listId, long itemId)
        {
            var item = await _context.TodoItem
                .Where(i => i.TodoListId == listId && i.Id == itemId)
                .FirstOrDefaultAsync();

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPut("{itemId}")]
        public async Task<ActionResult> UpdateItem(long listId, long itemId, UpdateTodoItem payload)
        {
            var item = await _context.TodoItem
                .Where(i => i.TodoListId == listId && i.Id == itemId)
                .FirstOrDefaultAsync();

            if (item == null)
                return NotFound();

            item.Description = payload.Description;
            await _context.SaveChangesAsync();

            return Ok(item);
        }

        [HttpPatch("{itemId}")]
        public async Task<ActionResult> CompleteItem(long listId, long itemId)
        {
            var item = await _context.TodoItem
                .Where(i => i.TodoListId == listId && i.Id == itemId)
                .FirstOrDefaultAsync();

            if (item == null)
                return NotFound();

            item.Completed = true;
            await _context.SaveChangesAsync();

            return Ok(item);
        }

        // DELETE: api/todolists/5/items/10
        [HttpDelete("{itemId}")]
        public async Task<ActionResult> DeleteItem(long listId, long itemId)
        {
            var item = await _context.TodoItem
                .Where(i => i.TodoListId == listId && i.Id == itemId)
                .FirstOrDefaultAsync();

            if (item == null)
                return NotFound();

            _context.TodoItem.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
