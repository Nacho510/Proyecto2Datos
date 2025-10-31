using Proyecto2Datos.Model;
using Proyecto2Datos.Services.Distancias;
using Proyecto2Datos.Services.Normalizacion;
using Proyecto2Datos.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto2Datos.Services
{
    public class ProcesadorCluster
    {
        private readonly ConfigSingleton config = ConfigSingleton.Instancia;
        private readonly List<Cluster> clusters = new();
        private Matriz matriz;
        private bool[] activos;

        public Cluster ClusterFinal { get; private set; }
        
        public ProcesadorCluster(List<DataPoint> puntos)
        {
            if (puntos == null || puntos.Count == 0)
                throw new ArgumentException("La lista de puntos está vacía.");

            foreach (var p in puntos)
            {
                var cluster = new Cluster(p);
                clusters.Add(cluster);
            }

            activos = new bool[clusters.Count];
            Array.Fill(activos, true);
        }

        public Cluster EjecutarClustering()
        {
            return EjecutarClusteringInterno(config.DistanciaStrategy);
        }

        private Cluster EjecutarClusteringInterno(IDistanciaStrategy distanciaStrategy)
        {
            int n = clusters.Count;
            matriz = new Matriz(n);

            Console.WriteLine($"🔄 Iniciando clustering con {n} elementos...");
            var inicio = DateTime.Now;

            CalcularMatrizInicial(distanciaStrategy);

            if (!config.ModoSilencioso)
                Console.WriteLine($"✓ Matriz de distancias calculada ({n}x{n})");

            int clustersActivos = n;
            int iter = 0;
            int siguienteIndice = n;

            while (clustersActivos > 1)
            {
                (int iMin, int jMin, float distMin) = EncontrarMinimoOptimizado();

                if (iMin == -1 || jMin == -1)
                {
                    Console.WriteLine("⚠ No se encontró mínimo válido, forzando fusión.");
                    (iMin, jMin, distMin) = ForzarFusion();
                }

                var c1 = clusters[iMin];
                var c2 = clusters[jMin];
                var nuevo = new Cluster(c1, c2, distMin); // crea nodo con hijos y distancia

                activos[iMin] = false;
                activos[jMin] = false;

                if (siguienteIndice >= clusters.Count)
                {
                    clusters.Add(nuevo);
                    Array.Resize(ref activos, clusters.Count);
                }
                else
                {
                    clusters[siguienteIndice] = nuevo;
                }

                activos[siguienteIndice] = true;

                ActualizarDistanciasNuevoCluster(siguienteIndice, distanciaStrategy);

                siguienteIndice++;
                clustersActivos--;
                iter++;

                if (!config.ModoSilencioso && (iter % 100 == 0 || clustersActivos < 20))
                {
                    var transcurrido = (DateTime.Now - inicio).TotalSeconds;
                    Console.WriteLine($"🧩 Iter {iter}: quedan {clustersActivos} clusters ({transcurrido:F1}s)");
                }
            }

            ClusterFinal = clusters.First(c => activos[clusters.IndexOf(c)]);

            var tiempoTotal = (DateTime.Now - inicio).TotalSeconds;
            if (!config.ModoSilencioso)
                Console.WriteLine($"✅ Clustering completado en {tiempoTotal:F2} segundos.");

            return ClusterFinal;
        }

        private void CalcularMatrizInicial(IDistanciaStrategy distancia)
        {
            int n = clusters.Count;

            if (n < 50)
            {
                for (int i = 0; i < n; i++)
                    for (int j = 0; j <= i; j++)
                        matriz.SetValor(i, j, distancia.Calcular(clusters[i].Centroide, clusters[j].Centroide));
            }
            else
            {
                Parallel.For(0, n, i =>
                {
                    for (int j = 0; j <= i; j++)
                    {
                        float d = distancia.Calcular(clusters[i].Centroide, clusters[j].Centroide);
                        if (float.IsNaN(d) || float.IsInfinity(d))
                            d = float.MaxValue;
                        matriz.SetValor(i, j, d);
                    }
                });
            }
        }

        private (int, int, float) EncontrarMinimoOptimizado()
        {
            float min = float.MaxValue;
            int iMin = -1, jMin = -1;

            for (int i = 0; i < clusters.Count; i++)
            {
                if (!activos[i]) continue;

                for (int j = 0; j < i; j++)
                {
                    if (!activos[j]) continue;

                    float val = matriz.GetValor(i, j);
                    if (val > 1e-6 && val < min && !float.IsNaN(val))
                    {
                        min = val;
                        iMin = i;
                        jMin = j;
                    }
                }
            }

            return (iMin, jMin, min);
        }

        private (int, int, float) ForzarFusion()
        {
            for (int i = 0; i < clusters.Count; i++)
            {
                if (!activos[i]) continue;
                for (int j = i + 1; j < clusters.Count; j++)
                {
                    if (activos[j])
                        return (i, j, 0.001f);
                }
            }
            return (-1, -1, 0);
        }

        private void ActualizarDistanciasNuevoCluster(int indiceNuevo, IDistanciaStrategy distancia)
        {
            if (indiceNuevo >= matriz.Filas)
            {
                var nuevaMatriz = new Matriz(indiceNuevo + 1);

                for (int i = 0; i < matriz.Filas; i++)
                    for (int j = 0; j <= i; j++)
                        nuevaMatriz.SetValor(i, j, matriz.GetValor(i, j));

                matriz = nuevaMatriz;
            }

            for (int i = 0; i < clusters.Count; i++)
            {
                if (!activos[i] || i == indiceNuevo) continue;

                float d = distancia.Calcular(clusters[indiceNuevo].Centroide, clusters[i].Centroide);
                if (float.IsNaN(d) || float.IsInfinity(d))
                    d = float.MaxValue;

                if (indiceNuevo > i)
                    matriz.SetValor(indiceNuevo, i, d);
                else
                    matriz.SetValor(i, indiceNuevo, d);
            }
        }
    }
}
