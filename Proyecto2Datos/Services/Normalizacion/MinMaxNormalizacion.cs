using Proyecto2Datos.Model;

namespace Proyecto2Datos.Services.Normalizacion
{
    public class MinMaxNormalizacion : INormalizacionStrategy
    {
        public string Nombre => "MinMax";

        public void Normalizar(List<DataPoint> puntos)
        {
            if (puntos == null || puntos.Count == 0)
            {
                Console.WriteLine("⚠ No hay puntos para normalizar");
                return;
            }

            int dimension = puntos[0].Vector.Dimension;
            float[] min = new float[dimension];
            float[] max = new float[dimension];
            
            // Inicializar con primer punto
            for (int i = 0; i < dimension; i++)
            {
                min[i] = puntos[0].Vector[i]; // ✅ Ya devuelve float
                max[i] = puntos[0].Vector[i]; // ✅ Ya devuelve float
            }

            // Encontrar min y max
            foreach (var p in puntos)
            {
                for (int i = 0; i < dimension; i++)
                {
                    float val = p.Vector[i];
                    
                    if (float.IsNaN(val) || float.IsInfinity(val))
                    {
                        p.Vector[i] = 0f;
                        continue;
                    }
                    
                    if (val < min[i]) min[i] = val;
                    if (val > max[i]) max[i] = val;
                }
            }

            // Normalizar
            int columnasConstantes = 0;
            foreach (var p in puntos)
            {
                for (int i = 0; i < dimension; i++)
                {
                    float denom = (max[i] - min[i]);
                    if (Math.Abs(denom) < 1e-10f)
                    {
                        p.Vector[i] = 0f;
                        if (columnasConstantes == 0)
                            columnasConstantes++;
                    }
                    else
                    {
                        p.Vector[i] = (p.Vector[i] - min[i]) / denom;
                    }
                }
            }

            if (columnasConstantes > 0)
            {
                Console.WriteLine($"⚠ Columnas constantes detectadas y normalizadas a 0");
            }
        }
    }
}