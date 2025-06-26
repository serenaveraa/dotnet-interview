using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http.Json;
using TodoApi.Dtos;
using TodoApi.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();

// Tool definitions (must be static)
[McpServerToolType]
public static class TodoTools
{
    private static readonly HttpClient http = new();

    [McpServerTool(Name = "CrearLista"), Description("Crea una lista de tareas con el nombre especificado.")]
    public static async Task<string> CrearLista([Description("Nombre de la lista")] string nombre)
    {
        var response = await http.PostAsJsonAsync("http://localhost:5083/api/todolists", new CreateTodoList { Name = nombre });
        response.EnsureSuccessStatusCode();
        var lista = await response.Content.ReadFromJsonAsync<TodoList>();
        return $"✅ Lista '{nombre}' creada con ID {lista!.Id}";
    }

    [McpServerTool(Name = "CrearItem"), Description("Crea un ítem en una lista existente.")]
    public static async Task<string> CrearItem(
        [Description("ID de la lista")] long listaId,
        [Description("Descripción del ítem")] string descripcion)
    {
        var response = await http.PostAsJsonAsync($"http://localhost:5083/api/todolists/{listaId}", new CreateTodoItem { Description = descripcion });
        response.EnsureSuccessStatusCode();
        var item = await response.Content.ReadFromJsonAsync<TodoItem>();
        return $"✅ Ítem creado en lista {listaId} con ID {item!.Id}";
    }

    [McpServerTool(Name = "CompletarItem"), Description("Marca un ítem como completado.")]
    public static async Task<string> CompletarItem(
        [Description("ID de la lista")] long listaId,
        [Description("ID del ítem")] long itemId)
    {
        try
        {
            var response = await http.PatchAsync($"http://localhost:5083/api/todolists/{listaId}/{itemId}", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"❌ Error completando el ítem {itemId} en lista {listaId}: {response.StatusCode} - {error}";
            }

            return $"✅ Ítem {itemId} marcado como completado.";
        }
        catch (Exception ex)
        {
            return $"❌ Excepción al completar ítem: {ex.Message}";
        }
    }


    [McpServerTool(Name = "ObtenerListas"), Description("Devuelve todas las listas de tareas.")]
    public static async Task<IList<TodoList>> ObtenerListas()
    {
        var listas = await http.GetFromJsonAsync<IList<TodoList>>("http://localhost:5083/api/todolists");
        return listas!;
    }
    
    // Devuelve un ítem por ID
[McpServerTool(Name = "ObtenerItem"), Description("Devuelve un ítem de una lista por ID.")]
public static async Task<TodoItem?> ObtenerItem(
    [Description("ID de la lista")] long listaId,
    [Description("ID del ítem")] long itemId)
{
    return await http.GetFromJsonAsync<TodoItem>($"http://localhost:5083/api/todolists/{listaId}/{itemId}");
}

// Actualiza un ítem
[McpServerTool(Name = "ActualizarItem"), Description("Actualiza la descripción de un ítem.")]
public static async Task<string> ActualizarItem(
    [Description("ID de la lista")] long listaId,
    [Description("ID del ítem")] long itemId,
    [Description("Nueva descripción")] string descripcion)
{
    var response = await http.PutAsJsonAsync(
        $"http://localhost:5083/api/todolists/{listaId}/{itemId}",
        new UpdateTodoItem { Description = descripcion });

    response.EnsureSuccessStatusCode();
    return $"📝 Ítem {itemId} actualizado con nueva descripción.";
}

// Elimina un ítem
[McpServerTool(Name = "EliminarItem"), Description("Elimina un ítem de una lista.")]
public static async Task<string> EliminarItem(
    [Description("ID de la lista")] long listaId,
    [Description("ID del ítem")] long itemId)
{
    var response = await http.DeleteAsync($"http://localhost:5083/api/todolists/{listaId}/{itemId}");
    response.EnsureSuccessStatusCode();
    return $"🗑️ Ítem {itemId} eliminado.";
}

// Obtener una lista por ID
[McpServerTool(Name = "ObtenerLista"), Description("Devuelve una lista de tareas con sus ítems.")]
public static async Task<TodoList?> ObtenerLista(
    [Description("ID de la lista")] long listaId)
{
    return await http.GetFromJsonAsync<TodoList>($"http://localhost:5083/api/todolists/{listaId}");
}

// Actualizar una lista
[McpServerTool(Name = "ActualizarLista"), Description("Actualiza el nombre de una lista.")]
public static async Task<string> ActualizarLista(
    [Description("ID de la lista")] long listaId,
    [Description("Nuevo nombre")] string nombre)
{
    var response = await http.PutAsJsonAsync(
        $"http://localhost:5083/api/todolists/{listaId}",
        new UpdateTodoList { Name = nombre });

    response.EnsureSuccessStatusCode();
    return $"📝 Lista {listaId} actualizada con nuevo nombre.";
}

// Eliminar una lista
[McpServerTool(Name = "EliminarLista"), Description("Elimina una lista de tareas.")]
public static async Task<string> EliminarLista(
    [Description("ID de la lista")] long listaId)
{
    var response = await http.DeleteAsync($"http://localhost:5083/api/todolists/{listaId}");
    response.EnsureSuccessStatusCode();
    return $"🗑️ Lista {listaId} eliminada.";
}

[McpServerTool(Name = "CrearItemPorNombreLista"), Description("Crea un ítem en una lista por su nombre.")]
public static async Task<string> CrearItemPorNombreLista(
    [Description("Nombre de la lista")] string nombreLista,
    [Description("Descripción del ítem")] string descripcion)
{
    var listas = await http.GetFromJsonAsync<IList<TodoList>>("http://localhost:5083/api/todolists");
    var lista = listas!.FirstOrDefault(l => l.Name.Equals(nombreLista, StringComparison.OrdinalIgnoreCase));

    if (lista == null)
        return $"❌ No se encontró la lista '{nombreLista}'.";

    var response = await http.PostAsJsonAsync($"http://localhost:5083/api/todolists/{lista.Id}", new CreateTodoItem { Description = descripcion });
    response.EnsureSuccessStatusCode();
    var item = await response.Content.ReadFromJsonAsync<TodoItem>();
    return $"✅ Ítem creado en lista '{nombreLista}' con ID {item!.Id}";
}


}
