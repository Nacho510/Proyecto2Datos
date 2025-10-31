using Proyecto2Datos.Model;

namespace Proyecto2Datos.Services.Distancias;

public class EuclidianaDist : IDistanciaStrategy
{
    public string Nombre => "Euclidiana";

    public float Calcular(Vector v1, Vector v2)
    {
        if (v1.Dimension != v2.Dimension)
            throw new ArgumentException("Los vectores tienen diferente dimensión");

        float suma = 0f;
        for (int i = 0; i < v1.Dimension; i++)
        {
            float diff = v1[i] - v2[i];
            suma += diff * diff;
        }
        return (float)Math.Sqrt(suma);
    }
}