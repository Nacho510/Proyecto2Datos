namespace Proyecto2Datos.Model;

public class ClusterIterador
{
    private readonly Cluster raiz;

    public ClusterIterador(Cluster raiz)
    {
        this.raiz = raiz;
    }

    public IEnumerable<Cluster> Recorrer()
    {
        var lista = new List<Cluster>();
        RecorrerRecursivo(raiz, lista);
        return lista;
    }

    private void RecorrerRecursivo(Cluster cluster, List<Cluster> lista)
    {
        lista.Add(cluster);
        foreach (var sub in cluster.SubClusters)
        {
            RecorrerRecursivo(sub, lista);
        }
    }

    private void ImprimirRecursivo(Cluster cluster, int nivel)
    {
        Console.WriteLine($"{new string(' ', nivel * 2)}Cluster {cluster.Id}");

        if (cluster.SubClusters.Count > 0)
        {
            foreach (var sub in cluster.SubClusters)
                ImprimirRecursivo(sub, nivel + 1);
        }
        else
        {
            foreach (var punto in cluster.Puntos)
                Console.WriteLine($"{new string(' ', (nivel + 1) * 2)}• {punto.Id}");
        }
    }
}