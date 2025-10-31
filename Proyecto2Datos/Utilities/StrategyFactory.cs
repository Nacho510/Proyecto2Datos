using Proyecto2Datos.Services.Normalizacion;
using Proyecto2Datos.Services.Distancias;

namespace Proyecto2Datos.Utilities;

public static class StrategyFactory
{
    public static IDistanciaStrategy CrearDistancia(string tipo)
    {
        return tipo.ToLower() switch
        {
            "euclidiana" => new EuclidianaDist(),
            "manhattan" => new ManhattanDist(),
            "coseno" => new CosenoDist(),
            "hamming" => new HammingDist(),
            _ => throw new ArgumentException($"Tipo de distancia '{tipo}' no reconocido.")
        };
    }

    public static INormalizacionStrategy CrearNormalizacion(string tipo)
    {
        return tipo.ToLower() switch
        {
            "minmax" => new MinMaxNormalizacion(),
            "zscore" => new ZScoreNormalizacion(),
            "log" or "logaritmica" => new LogNormalizacion(),
            _ => throw new ArgumentException($"Tipo de normalización '{tipo}' no reconocido.")
        };
    }
}