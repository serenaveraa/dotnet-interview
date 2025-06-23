using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Tests;

#nullable disable
public class TodoItemsControllerTests
{
    private DbContextOptions<TodoContext> DatabaseContextOptions()
    {
        return new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    private void PopulateDatabaseContext(TodoContext context)
    {
        var list1 = new TodoList { Id = 1, Name = "Lista 1" };
        var list2 = new TodoList { Id = 2, Name = "Lista 2" };
        context.TodoList.AddRange(list1, list2);

        context.TodoItem.AddRange(
            new TodoItem { Id = 1, TodoListId = 1, Description = "Item 1 en lista 1", Completed = false },
            new TodoItem { Id = 2, TodoListId = 1, Description = "Item 2 en lista 1", Completed = true },
            new TodoItem { Id = 3, TodoListId = 2, Description = "Item 1 en lista 2", Completed = false }
        );

        context.SaveChanges();
    }

    [Fact]
    public async Task CreateItem_WhenListExists_CreatesNewTodoItem()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        PopulateDatabaseContext(context);

        var controller = new TodoItemsController(context);

        var newItem = new CreateTodoItem { Description = "Nuevo ítem" };
        var result = await controller.CreateItem(1, newItem);

        Assert.IsType<CreatedAtActionResult>(result.Result);

        var createdItem = (result.Result as CreatedAtActionResult).Value as TodoItem;
        Assert.NotNull(createdItem);
        Assert.Equal("Nuevo ítem", createdItem.Description);
        Assert.Equal(1, createdItem.TodoListId);
    }

    [Fact]
    public async Task CreateItem_WhenListDoesNotExist_ReturnsNotFound()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        PopulateDatabaseContext(context);

        var controller = new TodoItemsController(context);

        var newItem = new CreateTodoItem { Description = "Ítem inválido" };
        var result = await controller.CreateItem(999, newItem);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetItem_WhenItemExists_ReturnsTodoItem()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        PopulateDatabaseContext(context);

        var controller = new TodoItemsController(context);

        var result = await controller.GetItem(1, 1);

        Assert.IsType<OkObjectResult>(result.Result);

        var item = (result.Result as OkObjectResult).Value as TodoItem;
        Assert.NotNull(item);
        Assert.Equal(1, item.Id);
        Assert.Equal(1, item.TodoListId);
    }

    [Fact]
    public async Task GetItem_WhenItemDoesNotExist_ReturnsNotFound()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        PopulateDatabaseContext(context);

        var controller = new TodoItemsController(context);

        var result = await controller.GetItem(1, 999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateItem_WhenItemExists_UpdatesDescription()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        PopulateDatabaseContext(context);

        var controller = new TodoItemsController(context);

        var update = new UpdateTodoItem { Description = "Descripción actualizada" };
        var result = await controller.UpdateItem(1, 1, update);

        Assert.IsType<OkObjectResult>(result);

        var item = await context.TodoItem.FindAsync(1L);
        Assert.Equal("Descripción actualizada", item.Description);
    }

    [Fact]
    public async Task UpdateItem_WhenItemDoesNotExist_ReturnsNotFound()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        PopulateDatabaseContext(context);

        var controller = new TodoItemsController(context);

        var update = new UpdateTodoItem { Description = "Cualquier cosa" };
        var result = await controller.UpdateItem(1, 999, update);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CompleteItem_WhenItemExists_SetsCompletedTrue()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        PopulateDatabaseContext(context);

        var controller = new TodoItemsController(context);

        var result = await controller.CompleteItem(1, 1);

        Assert.IsType<OkObjectResult>(result);

        var item = await context.TodoItem.FindAsync(1L);
        Assert.True(item.Completed);
    }

    [Fact]
    public async Task CompleteItem_WhenItemDoesNotExist_ReturnsNotFound()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        PopulateDatabaseContext(context);

        var controller = new TodoItemsController(context);

        var result = await controller.CompleteItem(1, 999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteItem_WhenItemExists_RemovesItem()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        PopulateDatabaseContext(context);

        var controller = new TodoItemsController(context);

        var result = await controller.DeleteItem(1, 1);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(await context.TodoItem.FindAsync(1L));
    }

    [Fact]
    public async Task DeleteItem_WhenItemDoesNotExist_ReturnsNotFound()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        PopulateDatabaseContext(context);

        var controller = new TodoItemsController(context);

        var result = await controller.DeleteItem(1, 999);

        Assert.IsType<NotFoundResult>(result);
    }
}

