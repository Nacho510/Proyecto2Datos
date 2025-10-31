using System.Text.Json;
using Proyecto2Datos.Model;

namespace Proyecto2Datos.Utilities;

public class JsonImportador
{
    public Resultado ImportarResultado(string ruta)
    {
        if (!File.Exists(ruta))
            throw new FileNotFoundException($"No se encontró el archivo JSON: {ruta}");

        string contenido = File.ReadAllText(ruta);
        var opciones = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
        };

        var resultado = JsonSerializer.Deserialize<Resultado>(contenido, opciones)
                        ?? throw new Exception("No se pudo deserializar el JSON.");

        return resultado;
    }
}