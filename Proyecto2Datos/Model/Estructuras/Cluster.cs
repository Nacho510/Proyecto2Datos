using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Proyecto2Datos.Model
{
    public class Cluster
    {
        [JsonPropertyName("n")]
        public string Nombre { get; set; }
        
        [JsonPropertyName("d")]
        public double Distancia { get; set; }
        
        [JsonPropertyName("c")]
        public List<Cluster> Hijos { get; set; } = new();
        
        public Vector Centroide { get; private set; }
        public int Tamaño { get; private set; }
        
        [JsonIgnore] public string Id => Nombre;
        [JsonIgnore] public List<Cluster> SubClusters => Hijos;
        [JsonIgnore] public List<DataPoint> Puntos { get; set; } = new();
        
        public Cluster(DataPoint punto)
        {
            Nombre = punto.Id.ToString();
            Distancia = 0;
            Hijos = new List<Cluster>();
            Centroide = punto.VectorDatos;
            Tamaño = 1;
            Puntos.Add(punto);
        }

        public Cluster(Cluster c1, Cluster c2, double distancia)
        {
            Nombre = $"({c1.Nombre},{c2.Nombre})";
            Distancia = Math.Round(distancia, 2); // ✅ Redondear como en test.json
            Hijos = new List<Cluster> { c1, c2 };
            Centroide = CalcularCentroide(c1.Centroide, c2.Centroide, c1.Tamaño, c2.Tamaño);
            Tamaño = c1.Tamaño + c2.Tamaño;

            // Combinar puntos de ambos clusters
            Puntos.AddRange(c1.Puntos);
            Puntos.AddRange(c2.Puntos);
        }

        public Cluster(string nombre, double distancia = 0)
        {
            Nombre = nombre;
            Distancia = distancia;
            Hijos = new List<Cluster>();
            Centroide = new Vector(1); // ✅ CORREGIDO: Ahora pasa int
            Tamaño = 1;
        }

        private Vector CalcularCentroide(Vector v1, Vector v2, int t1, int t2)
        {
            int dim = v1.Dimension; // ✅ CORREGIDO: era float[] ahora es int
            var nuevo = new Vector(dim);

            for (int i = 0; i < dim; i++)
                nuevo[i] = (v1[i] * t1 + v2[i] * t2) / (t1 + t2);

            return nuevo;
        }

        public bool EsHoja() => Hijos.Count == 0;
    }
}