namespace Proyecto2Datos.Model
{
    public class Resultado
    {
        public Cluster ClusterRaiz { get; set; }

        public Resultado(Cluster raiz)
        {
            ClusterRaiz = raiz;
        }

        public void MostrarEstructura()
        {
            Console.WriteLine("Estructura del dendrograma:");
            MostrarRecursivo(ClusterRaiz, 0);
        }

        private void MostrarRecursivo(Cluster cluster, int nivel)
        {
            Console.WriteLine($"{new string(' ', nivel * 2)}- Cluster {cluster.Id} ({cluster.Puntos.Count} puntos)");

            if (cluster.SubClusters != null && cluster.SubClusters.Count > 0)
            {
                foreach (var sub in cluster.SubClusters)
                    MostrarRecursivo(sub, nivel + 1);
            }
            else if (cluster.Puntos != null && cluster.Puntos.Count <= 5)
            {
                foreach (var p in cluster.Puntos)
                    Console.WriteLine($"{new string(' ', (nivel + 1) * 2)}• {p.Id}");
            }
        }

        // Método estático requerido por Controller
        public static void Imprimir(Resultado resultado)
        {
            if (resultado == null)
            {
                Console.WriteLine("⚠ No hay resultado para mostrar.");
                return;
            }

            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("RESULTADO DEL CLUSTERING JERÁRQUICO");
            Console.WriteLine(new string('=', 50));
            
            resultado.MostrarEstructura();
            
            Console.WriteLine(new string('=', 50));
            Console.WriteLine($"Total de puntos agrupados: {resultado.ClusterRaiz.Puntos.Count}");
            Console.WriteLine(new string('=', 50) + "\n");
        }

        public void MostrarEstadisticas()
        {
            int totalClusters = ContarClusters(ClusterRaiz);
            int profundidadMaxima = ObtenerProfundidad(ClusterRaiz);
            
            Console.WriteLine("\n📊 Estadísticas del dendrograma:");
            Console.WriteLine($"   • Total de clusters: {totalClusters}");
            Console.WriteLine($"   • Profundidad máxima: {profundidadMaxima}");
            Console.WriteLine($"   • Total de puntos: {ClusterRaiz.Puntos.Count}");
        }

        private int ContarClusters(Cluster cluster)
        {
            if (cluster == null) return 0;
            
            int count = 1;
            foreach (var sub in cluster.SubClusters)
                count += ContarClusters(sub);
            
            return count;
        }

        private int ObtenerProfundidad(Cluster cluster, int nivel = 0)
        {
            if (cluster == null || cluster.SubClusters.Count == 0)
                return nivel;
            
            int maxProfundidad = nivel;
            foreach (var sub in cluster.SubClusters)
            {
                int profundidad = ObtenerProfundidad(sub, nivel + 1);
                if (profundidad > maxProfundidad)
                    maxProfundidad = profundidad;
            }
            
            return maxProfundidad;
        }
    }
}