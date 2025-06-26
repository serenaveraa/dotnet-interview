using ModelContextProtocol.Client;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using McpClient;
using ModelContextProtocol.Protocol;

var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "TodoServer",
    Command = "dotnet",
    Arguments = ["run", "--project", "McpServer"],
});

var client = await McpClientFactory.CreateAsync(clientTransport);

var tools = await client.ListToolsAsync();

while (true)
{
    Console.Write("Prompt > ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) break;

    var parsed = PromptInterpreter.Interpret(input!);
    if (parsed == null)
    {
        Console.WriteLine("❌ No se pudo interpretar el prompt.");
        continue;
    }

    var (toolName, parameters) = parsed.Value;

    Console.WriteLine($"🔧 Invocando tool: {toolName}");
    var result = await client.CallToolAsync(toolName, parameters);

    var textBlock = result.Content.FirstOrDefault(c => c is TextContentBlock) as TextContentBlock;
    if (textBlock != null)
        Console.WriteLine($"🟢 Resultado: {textBlock.Text}");
    else
        Console.WriteLine("🟢 Resultado: (sin contenido de texto)");
}
