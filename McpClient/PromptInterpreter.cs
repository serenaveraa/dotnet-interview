using System.Text.RegularExpressions;

namespace McpClient;

public class PromptInterpreter
{
    public static (string ToolName, Dictionary<string, object?> Parameters)? Interpret(string prompt)
    {
        prompt = prompt.ToLowerInvariant();

        // === Crear Ítem ===
        if (prompt.Contains("crear") && (prompt.Contains("item") || prompt.Contains("ítem")))
        {
            // Buscar nombre lista (palabras después de "lista" o "en lista")
            string? lista = null;
            var listaMatch = Regex.Match(prompt, @"lista\s+([^\s""]+)", RegexOptions.IgnoreCase);
            if (listaMatch.Success)
                lista = listaMatch.Groups[1].Value;

            // Buscar descripción (descripción o descripcion) entre comillas o tras "descripcion" o "con descripcion"
            string? descripcion = null;
            // 1) texto entre comillas
            var descMatch1 = Regex.Match(prompt, @"descripcion\s+""([^""]+)""", RegexOptions.IgnoreCase);
            if (descMatch1.Success)
                descripcion = descMatch1.Groups[1].Value;
            else
            {
                // 2) texto tras 'descripcion' sin comillas, hasta fin o hasta 'en lista' u otra palabra clave
                var descMatch2 = Regex.Match(prompt, @"descripcion\s+([^\s]+)", RegexOptions.IgnoreCase);
                if (descMatch2.Success)
                    descripcion = descMatch2.Groups[1].Value;
                else
                {
                    // 3) buscar tras "con descripcion"
                    var descMatch3 = Regex.Match(prompt, @"con descripcion\s+([^\s]+)", RegexOptions.IgnoreCase);
                    if (descMatch3.Success)
                        descripcion = descMatch3.Groups[1].Value;
                }
            }

            if (!string.IsNullOrWhiteSpace(lista) && !string.IsNullOrWhiteSpace(descripcion))
            {
                return ("CrearItemPorNombreLista", new Dictionary<string, object?>
                {
                    ["nombreLista"] = lista,
                    ["descripcion"] = descripcion
                });
            }
        }

        // === Crear Lista ===
        if (prompt.Contains("crear") && prompt.Contains("lista"))
        {
            // Buscar nombre de lista (palabra después de 'crear lista' o 'lista')
            var listaMatch = Regex.Match(prompt, @"crear lista\s+([^\s""]+)", RegexOptions.IgnoreCase);
            if (!listaMatch.Success)
                listaMatch = Regex.Match(prompt, @"lista\s+([^\s""]+)", RegexOptions.IgnoreCase);

            var lista = listaMatch.Success ? listaMatch.Groups[1].Value : null;

            if (!string.IsNullOrWhiteSpace(lista))
            {
                return ("CrearLista", new Dictionary<string, object?>
                {
                    ["nombre"] = lista
                });
            }
        }

        // === Completar Ítem (ejemplo simple) ===
        if (prompt.Contains("completar") && prompt.Contains("item"))
        {
            // Buscar IDs (números)
            var ids = Regex.Matches(prompt, @"\d+").Select(m => long.Parse(m.Value)).ToList();
            if (ids.Count >= 2)
            {
                return ("CompletarItem", new Dictionary<string, object?>
                {
                    ["listaId"] = ids[0],
                    ["itemId"] = ids[1]
                });
            }
        }

        // === Obtener listas ===
        if (prompt.Contains("obtener") && prompt.Contains("listas"))
        {
            return ("ObtenerListas", new Dictionary<string, object?>());
        }

        return null;
    }
}
