using Proyecto2Datos.Services.Distancias;
using Proyecto2Datos.Services.Normalizacion;

namespace Proyecto2Datos.Utilities
{
    public sealed class ConfigSingleton
    {
        private static readonly Lazy<ConfigSingleton> instancia = new(() => new ConfigSingleton());
        public static ConfigSingleton Instancia => instancia.Value;

        private ConfigSingleton() { }

        public string RutaCSV { get; set; } = "";
        public string MetodoDistancia { get; set; } = "Euclidiana";
        public string MetodoNormalizacion { get; set; } = "MinMax";
        public double[]? PesosVariables { get; set; } = null;

        public IDistanciaStrategy DistanciaStrategy { get; set; }
        public INormalizacionStrategy NormalizacionStrategy { get; set; }

        public bool ModoSilencioso { get; set; } = false;

        public void InicializarEstrategias()
        {
            if (string.IsNullOrWhiteSpace(MetodoDistancia))
            {
                Console.WriteLine("⚠ Método de distancia no definido, usando Euclidiana por defecto");
                MetodoDistancia = "Euclidiana";
            }

            if (string.IsNullOrWhiteSpace(MetodoNormalizacion))
            {
                Console.WriteLine("⚠ Método de normalización no definido, usando MinMax por defecto");
                MetodoNormalizacion = "MinMax";
            }

            DistanciaStrategy = MetodoDistancia.ToLower() switch
            {
                "manhattan" => new ManhattanDist(),
                "hamming" => new HammingDist(),
                "coseno" => new CosenoDist(),
                _ => new EuclidianaDist()
            };

            NormalizacionStrategy = MetodoNormalizacion.ToLower() switch
            {
                "zscore" => new ZScoreNormalizacion(),
                "log" => new LogNormalizacion(),
                _ => new MinMaxNormalizacion()
            };

            if (!ModoSilencioso)
            {
                Console.WriteLine($"✓ Estrategias inicializadas:");
                Console.WriteLine($"  - Distancia: {DistanciaStrategy.Nombre}");
                Console.WriteLine($"  - Normalización: {NormalizacionStrategy.Nombre}");
            }
        }
    }
}