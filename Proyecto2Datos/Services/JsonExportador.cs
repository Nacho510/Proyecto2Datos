using System.IO;
using System.Text.Json;
using Proyecto2Datos.Model;

namespace Proyecto2Datos.Utilities;

public class JsonExportador
{
    public static void ExportarDendrograma(Cluster raiz, string rutaArchivo)
    {
        var opciones = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = null // importante: respeta "n", "d", "c"
        };

        string json = JsonSerializer.Serialize(raiz, opciones);
        File.WriteAllText(rutaArchivo, json);
    }
}