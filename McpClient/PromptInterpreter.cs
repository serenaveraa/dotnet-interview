using System.Text.RegularExpressions;

namespace McpClient;

public class PromptInterpreter
{
    public static (string ToolName, Dictionary<string, object?> Parameters)? Interpret(string prompt)
    {
        prompt = prompt.ToLowerInvariant();
        
        if (prompt.Contains("crear") && (prompt.Contains("item") || prompt.Contains("ítem")))
        {
            string? lista = null;
            var listaMatch = Regex.Match(prompt, @"lista\s+([^\s""]+)", RegexOptions.IgnoreCase);
            if (listaMatch.Success)
                lista = listaMatch.Groups[1].Value;

            string? descripcion = null;
            var descMatch1 = Regex.Match(prompt, @"descripcion\s+""([^""]+)""", RegexOptions.IgnoreCase);
            if (descMatch1.Success)
                descripcion = descMatch1.Groups[1].Value;
            else
            {
                var descMatch2 = Regex.Match(prompt, @"descripcion\s+([^\s]+)", RegexOptions.IgnoreCase);
                if (descMatch2.Success)
                    descripcion = descMatch2.Groups[1].Value;
                else
                {
                    var descMatch3 = Regex.Match(prompt, @"con descripcion\s+(.+)", RegexOptions.IgnoreCase);
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
        
        if (prompt.Contains("crear") && prompt.Contains("lista"))
        {
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
        
        if (prompt.Contains("completar") && prompt.Contains("item"))
        {
            // Busca expresiones como: "item 7 de la lista 1"
            var match = Regex.Match(prompt, @"item\s+(\d+)\s+(de|en)\s+la\s+lista\s+(\d+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                long itemId = long.Parse(match.Groups[1].Value);
                long listaId = long.Parse(match.Groups[3].Value);

                return ("CompletarItem", new Dictionary<string, object?>
                {
                    ["listaId"] = listaId,
                    ["itemId"] = itemId
                });
            }
        }
        
        if ((prompt.Contains("obtener") || prompt.Contains("ver")) && prompt.Contains("listas"))
        {
            return ("ObtenerListas", new Dictionary<string, object?>());
        }
        
        if ((prompt.Contains("obtener") || prompt.Contains("ver") || prompt.Contains("mostrar")) &&
            prompt.Contains("item"))
        {
            var ids = Regex.Matches(prompt, @"\d+").Select(m => long.Parse(m.Value)).ToList();
            if (ids.Count >= 2)
            {
                return ("ObtenerItem", new Dictionary<string, object?>
                {
                    ["listaId"] = ids[1],
                    ["itemId"] = ids[0]
                });
            }
        }
        
        if ((prompt.Contains("obtener") || prompt.Contains("ver") || prompt.Contains("mostrar") || prompt.Contains("obtener") ) && prompt.Contains("lista"))
        {
            var idMatch = Regex.Match(prompt, @"lista\s+(\d+)", RegexOptions.IgnoreCase);
            if (idMatch.Success)
            {
                return ("ObtenerLista", new Dictionary<string, object?>
                {
                    ["listaId"] = long.Parse(idMatch.Groups[1].Value)
                });
            }
        }
        
        if (prompt.Contains("actualizar") && prompt.Contains("item"))
        {
            var match = Regex.Match(prompt, @"item\s+(\d+)\s+de\s+la\s+lista\s+(\d+).*descripcion\s+(.+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return ("ActualizarItem", new Dictionary<string, object?>
                {
                    ["itemId"] = long.Parse(match.Groups[1].Value),
                    ["listaId"] = long.Parse(match.Groups[2].Value),
                    ["descripcion"] = match.Groups[3].Value.Trim()
                });
            }
        }
        
        if ((prompt.Contains("eliminar") || prompt.Contains("borrar")) && prompt.Contains("item"))
        {
            var ids = Regex.Matches(prompt, @"\d+").Select(m => long.Parse(m.Value)).ToList();
            if (ids.Count >= 2)
            {
                return ("EliminarItem", new Dictionary<string, object?>
                {
                    ["listaId"] = ids[1],
                    ["itemId"] = ids[0]
                });
            }
        }
        
        if ((prompt.Contains("actualizar") || prompt.Contains("editar")) && prompt.Contains("lista"))
        {
            var match = Regex.Match(prompt, @"lista\s+(\d+).*nombre\s+(.+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return ("ActualizarLista", new Dictionary<string, object?>
                {
                    ["listaId"] = long.Parse(match.Groups[1].Value),
                    ["nombre"] = match.Groups[2].Value.Trim()
                });
            }
        }
        
        if ((prompt.Contains("eliminar") || prompt.Contains("borrar")) && prompt.Contains("lista"))
        {
            var match = Regex.Match(prompt, @"lista\s+(\d+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return ("EliminarLista", new Dictionary<string, object?>
                {
                    ["listaId"] = long.Parse(match.Groups[1].Value)
                });
            }
        }

        return null;
    }
}
