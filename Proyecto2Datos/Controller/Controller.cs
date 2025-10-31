using Proyecto2Datos.Model;
using Proyecto2Datos.Services;
using Proyecto2Datos.Utilities;
using System;
using System.Collections.Generic;
using Proyecto2Datos.Services.Distancias;
using Proyecto2Datos.Services.Normalizacion;

namespace Proyecto2Datos.Controller
{
    public class Controller
    {
        private List<DataPoint> dataPoints;
        private ProcesadorCluster procesador;
        private LectorCSV lector;
        private Resultado resultado;
        private readonly ConfigSingleton config;

        public Controller()
        {
            dataPoints = new List<DataPoint>();
            lector = new LectorCSV();
            config = ConfigSingleton.Instancia;
        }

        public void CargarDatos(string rutaArchivo, int filasIgnorar = 0)
        {
            dataPoints = lector.LeerArchivo(rutaArchivo);
            Console.WriteLine($"Se cargaron {dataPoints.Count} registros desde {rutaArchivo}");
        }

        public void NormalizarDatos()
        {
            if (dataPoints.Count == 0)
            {
                Console.WriteLine("No hay datos cargados para normalizar.");
                return;
            }

            if (config.NormalizacionStrategy == null)
            {
                Console.WriteLine("⚠ No hay estrategia de normalización configurada. Usando MinMax por defecto.");
                config.MetodoNormalizacion = "MinMax";
                config.InicializarEstrategias();
            }

            // Aplicar pesos si están configurados
            if (config.PesosVariables != null && config.PesosVariables.Length > 0)
            {
                AplicarPesos();
            }

            config.NormalizacionStrategy.Normalizar(dataPoints);
            Console.WriteLine($"✓ Datos normalizados usando {config.NormalizacionStrategy.Nombre}");
        }

        private void AplicarPesos()
        {
            int dimension = dataPoints[0].Vector.Dimension;
            
            // Si hay menos pesos que dimensiones, rellenar con 1.0
            double[] pesos = new double[dimension];
            for (int i = 0; i < dimension; i++)
            {
                pesos[i] = i < config.PesosVariables.Length ? config.PesosVariables[i] : 1.0;
            }

            // Aplicar pesos a cada vector
            foreach (var punto in dataPoints)
            {
                for (int i = 0; i < dimension; i++)
                {
                    punto.Vector[i] *= (float)pesos[i];
                }
            }

            Console.WriteLine($"✓ Pesos aplicados: {string.Join(", ", pesos)}");
        }

        public void ProcesarCluster()
        {
            if (dataPoints.Count == 0)
            {
                Console.WriteLine("Primero debe cargar los datos.");
                return;
            }

            if (config.DistanciaStrategy == null)
            {
                Console.WriteLine("⚠ No hay estrategia de distancia configurada. Usando Euclidiana por defecto.");
                config.MetodoDistancia = "Euclidiana";
                config.InicializarEstrategias();
            }

            procesador = new ProcesadorCluster(dataPoints);
            Cluster clusterFinal = procesador.EjecutarClustering();
            resultado = new Resultado(clusterFinal);
            
            Console.WriteLine("✅ Clustering completado.");
        }

        public Resultado ObtenerResultado()
        {
            return resultado;
        }

        public void ExportarJson(string rutaArchivo)
        {
            if (resultado == null)
            {
                throw new Exception("No hay resultado para exportar. Ejecute el clustering primero.");
            }

            var exportador = new JsonExportador();
            JsonExportador.ExportarDendrograma(resultado.ClusterRaiz, rutaArchivo);
        }
    }
}