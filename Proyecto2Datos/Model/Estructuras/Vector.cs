using System;
using System.Linq;

namespace Proyecto2Datos.Model
{
    public class Vector
    {
        private float[] componentes;
        public int Dimension { get; private set; }

        public Vector(int dimension)
        {
            if (dimension <= 0)
                throw new ArgumentException("La dimensión debe ser mayor a 0");
            
            Dimension = dimension;
            componentes = new float[dimension];
        }

        public float this[int index]
        {
            get 
            { 
                if (index < 0 || index >= Dimension)
                    throw new IndexOutOfRangeException($"Índice {index} fuera de rango [0, {Dimension})");
                return componentes[index]; 
            }
            set 
            { 
                if (index < 0 || index >= Dimension)
                    throw new IndexOutOfRangeException($"Índice {index} fuera de rango [0, {Dimension})");
                componentes[index] = value; 
            }
        }

        public double ProductoPunto(Vector otro)
        {
            if (otro.Dimension != this.Dimension)
                throw new ArgumentException("Los vectores deben tener la misma dimensión.");

            double resultado = 0;
            for (int i = 0; i < Dimension; i++)
                resultado += this.componentes[i] * otro.componentes[i];

            return resultado;
        }

        public override string ToString()
        {
            return "[" + string.Join(",", componentes.Select(c => c.ToString("F3"))) + "]";
        }
    }
}