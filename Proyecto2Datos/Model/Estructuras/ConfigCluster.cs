using Proyecto2Datos.Services.Distancias;
using Proyecto2Datos.Services.Normalizacion;

namespace Proyecto2Datos.Model;

public class ConfigCluster
{
    public string MetodoDistancia { get; set; }
    public string MetodoNormalizacion { get; set; }
    public double[] PesosVariables { get; set; }

    public IDistanciaStrategy? DistanciaStrategy { get; set; }
    public INormalizacionStrategy? NormalizacionStrategy { get; set; }

    public ConfigCluster(string metodoDist, string metodoNorm, double[] pesos)
    {
        MetodoDistancia = metodoDist;
        MetodoNormalizacion = metodoNorm;
        PesosVariables = pesos;
    }
}