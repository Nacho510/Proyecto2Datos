using Proyecto2Datos.Model;

namespace Proyecto2Datos.Services.Distancias;

public class CosenoDist : IDistanciaStrategy
{
    public string Nombre => "Coseno";

    public float Calcular(Vector v1, Vector v2)
    {
        if (v1.Dimension != v2.Dimension)
            throw new ArgumentException("Los vectores tienen diferente dimensión");

        float producto = 0f, normaA = 0f, normaB = 0f;
        for (int i = 0; i < v1.Dimension; i++)
        {
            producto += v1[i] * v2[i];
            normaA += v1[i] * v1[i];
            normaB += v2[i] * v2[i];
        }

        if (normaA == 0 || normaB == 0)
            return 1f;

        float similitud = producto / ((float)Math.Sqrt(normaA) * (float)Math.Sqrt(normaB));
        float distancia = 1f - similitud;
        if (distancia < 0f) distancia = 0f; // por precisión
        return distancia;
    }
}