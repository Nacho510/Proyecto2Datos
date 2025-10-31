using Proyecto2Datos.Model;
using Proyecto2Datos.Utilities;

namespace Proyecto2Datos.Services.Normalizacion;

public class ZScoreNormalizacion : INormalizacionStrategy
{
    public string Nombre => "ZScore";

    public void Normalizar(List<DataPoint> puntos)
    {
        if (puntos == null || puntos.Count == 0) return;

        int d = puntos[0].Vector.Dimension;
        float[] medias = new float[d];
        float[] desv = new float[d];

        // calcular medias
        foreach (var p in puntos)
        {
            for (int i = 0; i < d; i++) medias[i] += p.Vector[i];
        }
        for (int i = 0; i < d; i++) medias[i] /= puntos.Count;

        // calcular desviaciones
        foreach (var p in puntos)
        {
            for (int i = 0; i < d; i++)
            {
                float diff = p.Vector[i] - medias[i];
                desv[i] += diff * diff;
            }
        }
        for (int i = 0; i < d; i++)
        {
            desv[i] = (float)Math.Sqrt(desv[i] / puntos.Count);
            if (desv[i] == 0) desv[i] = 1f;
        }

        // aplicar normalización
        foreach (var p in puntos)
        {
            for (int i = 0; i < d; i++)
                p.Vector[i] = (p.Vector[i] - medias[i]) / desv[i];
        }
    }
}