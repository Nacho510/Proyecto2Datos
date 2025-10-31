using System;

namespace Proyecto2Datos.Model
{
    public class Matriz
    {
        private float[][] datos;
        private int filas;
        
        public int Filas => filas;
        public int Columnas => filas;

        public Matriz(int n)
        {
            if (n <= 0) throw new ArgumentException("n debe ser > 0");
            filas = n;
            datos = new float[n][];
            for (int i = 0; i < n; i++)
                datos[i] = new float[i + 1];
        }

        public float GetValor(int i, int j)
        {
            if (i < 0 || j < 0 || i >= Filas || j >= Columnas)
                throw new IndexOutOfRangeException($"Índice fuera de rango: ({i}, {j})");
            
            return i >= j ? datos[i][j] : datos[j][i];
        }

        public void SetValor(int i, int j, float valor)
        {
            if (i < 0 || j < 0 || i >= Filas || j >= Columnas)
                throw new IndexOutOfRangeException($"Índice fuera de rango: ({i}, {j})");
            
            if (i >= j)
                datos[i][j] = valor;
            else
                datos[j][i] = valor;
        }

        public void PrintMatriz()
        {
            Console.WriteLine("\nMatriz de distancias:");
            for (int i = 0; i < Math.Min(10, Filas); i++)
            {
                for (int j = 0; j < Math.Min(10, Columnas); j++)
                    Console.Write($"{GetValor(i, j):F3}\t");
                Console.WriteLine();
            }
            if (Filas > 10)
                Console.WriteLine($"... ({Filas - 10} filas más)");
        }
    }
}