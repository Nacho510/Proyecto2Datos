using Proyecto2Datos.Model;


namespace Proyecto2Datos.Services.Normalizacion;

public class LogNormalizacion : INormalizacionStrategy
{
    public string Nombre => "Log";

    public void Normalizar(List<DataPoint> puntos)
    {
        if (puntos == null || puntos.Count == 0) return;

        int d = puntos[0].Vector.Dimension;
        foreach (var p in puntos)
        {
            for (int i = 0; i < d; i++)
            {
                float val = p.Vector[i];
                if (val < 0) val = 0;
                p.Vector[i] = (float)Math.Log10(val + 1f);
            }
        }
    }
}