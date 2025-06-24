using ModelContextProtocol.Client;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "TodoServer",
    Command = "dotnet",
    Arguments = ["run", "--project", "McpServer"],
});

var client = await McpClientFactory.CreateAsync(clientTransport);

// List tools from server
var tools = await client.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"🛠 {tool.Name}: {tool.Description}");
}

// Example: Call CrearItem
var result = await client.CallToolAsync(
    "CrearItem",
    new Dictionary<string, object?>
    {
        ["listaId"] = 1,
        ["descripcion"] = "Terminar informe"
    },
    cancellationToken: CancellationToken.None
);

// Show result
Console.WriteLine("\n🧠 Respuesta:");
Console.WriteLine(result.Content.First(c => c.Type == "text"));
