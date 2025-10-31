using Proyecto2Datos.Model;

namespace Proyecto2Datos.Services.Distancias;

    public class ManhattanDist : IDistanciaStrategy
    {
        public string Nombre => "Manhattan";

        public float Calcular(Vector v1, Vector v2)
        {
            if (v1.Dimension != v2.Dimension)
                throw new ArgumentException("Los vectores tienen diferente dimensión");

            float suma = 0f;
            for (int i = 0; i < v1.Dimension; i++)
                suma += Math.Abs(v1[i] - v2[i]);

            return suma;
        }
    }