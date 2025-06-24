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
        var response = await http.PatchAsync($"http://localhost:5083/api/todolists/{listaId}/{itemId}", null);
        response.EnsureSuccessStatusCode();
        return $"✅ Ítem {itemId} marcado como completado.";
    }

    [McpServerTool(Name = "ObtenerListas"), Description("Devuelve todas las listas de tareas.")]
    public static async Task<IList<TodoList>> ObtenerListas()
    {
        var listas = await http.GetFromJsonAsync<IList<TodoList>>("http://localhost:5083/api/todolists");
        return listas!;
    }
}
