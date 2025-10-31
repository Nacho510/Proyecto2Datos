using Proyecto2Datos.Model;

namespace Proyecto2Datos.Services.Distancias
{
    public class HammingDist : IDistanciaStrategy
    {
        public string Nombre => "Hamming";

        public float Calcular(Vector v1, Vector v2)
        {
            if (v1.Dimension != v2.Dimension)
                throw new ArgumentException("Los vectores tienen diferente dimensión");

            int diferencias = 0;
            for (int i = 0; i < v1.Dimension; i++)
            {
                // Comparar valores redondeados (para datos binarios/discretos)
                if (Math.Round(v1[i]) != Math.Round(v2[i]))
                    diferencias++;
            }

            // Retornar proporción de diferencias
            return (float)diferencias / v1.Dimension;
        }
    }
}