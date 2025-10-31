using Proyecto2Datos.Controller;
using Proyecto2Datos.Utilities;
using System;
using System.IO;

namespace Proyecto2Datos.View
{
    public class MenuConsola
    {
        private readonly Controller.Controller controller;
        private readonly ConfigSingleton config;

        public MenuConsola()
        {
            controller = new Controller.Controller();
            config = ConfigSingleton.Instancia;
        }

        public void Iniciar()
        {
            bool salir = false;

            while (!salir)
            {
                MostrarMenuPrincipal();
                Console.Write("\nSeleccione una opción: ");
                string opcion = Console.ReadLine() ?? "";

                switch (opcion)
                {
                    case "1":
                        ConfigurarArchivoCSV();
                        break;
                    case "2":
                        ConfigurarDistancia();
                        break;
                    case "3":
                        ConfigurarNormalizacion();
                        break;
                    case "4":
                        ConfigurarPesos();
                        break;
                    case "5":
                        EjecutarClustering();
                        break;
                    case "6":
                        MostrarResultado();
                        break;
                    case "7":
                        ExportarResultado();
                        break;
                    case "0":
                        salir = true;
                        Console.WriteLine("\n👋 Saliendo del sistema...");
                        break;
                    default:
                        Console.WriteLine("❌ Opción inválida. Intente nuevamente.");
                        break;
                }

                if (!salir)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        private void MostrarMenuPrincipal()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════╗");
            Console.WriteLine("║   SISTEMA DE CLUSTERING JERÁRQUICO - PROYECTO 2  ║");
            Console.WriteLine("╚══════════════════════════════════════════════════╝");
            Console.WriteLine("Archivo CSV actual: " + (string.IsNullOrWhiteSpace(config.RutaCSV) ? "No definido" : config.RutaCSV));
            Console.WriteLine($"Distancia: {config.MetodoDistancia ?? "No configurada"}");
            Console.WriteLine($"Normalización: {config.MetodoNormalizacion ?? "No configurada"}");
            Console.WriteLine($"Pesos: {(config.PesosVariables == null ? "No definidos" : string.Join(",", config.PesosVariables))}");
            Console.WriteLine("──────────────────────────────────────────────────");
            Console.WriteLine("1. Configurar archivo CSV");
            Console.WriteLine("2. Seleccionar tipo de distancia");
            Console.WriteLine("3. Seleccionar tipo de normalización");
            Console.WriteLine("4. Configurar pesos de variables");
            Console.WriteLine("5. Ejecutar clustering");
            Console.WriteLine("6. Mostrar estructura del dendrograma");
            Console.WriteLine("7. Exportar resultado a JSON");
            Console.WriteLine("0. Salir");
            Console.WriteLine("──────────────────────────────────────────────────");
        }

        private void ConfigurarArchivoCSV()
        {
            Console.Write("\n📂 Ingrese la ruta del archivo CSV");
            Console.Write("\n   (o Enter para usar '../../../datos/datos.csv'): ");
            
            string ruta = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(ruta))
                ruta = "C:\\Users\\ignab\\Downloads\\datos.csv";

            // Validar que el archivo existe
            if (!File.Exists(ruta))
            {
                Console.WriteLine($"\n⚠ ADVERTENCIA: El archivo NO existe en: {ruta}");
                Console.WriteLine("   El archivo será validado al cargar los datos.");
                Console.WriteLine("   ¿Desea intentar con otra ruta? (s/n): ");
                
                if (Console.ReadLine()?.ToLower() == "s")
                {
                    ConfigurarArchivoCSV();
                    return;
                }
            }
            else
            {
                var info = new FileInfo(ruta);
                Console.WriteLine($"\n✓ Archivo encontrado:");
                Console.WriteLine($"   • Tamaño: {info.Length / 1024.0:F2} KB");
                Console.WriteLine($"   • Última modificación: {info.LastWriteTime}");
            }

            config.RutaCSV = ruta;
            Console.WriteLine($"\n✓ Ruta configurada: {ruta}");
        }

        private void ConfigurarDistancia()
        {
            Console.WriteLine("\n📏 Tipos de distancia disponibles:");
            string[] distancias = { "Euclidiana", "Manhattan", "Hamming", "Coseno" };
            for (int i = 0; i < distancias.Length; i++)
                Console.WriteLine($"  {i + 1}. {distancias[i]}");

            Console.Write("\n→ Seleccione una opción (1-4): ");
            if (int.TryParse(Console.ReadLine(), out int op) && op >= 1 && op <= 4)
            {
                config.MetodoDistancia = distancias[op - 1];
                config.InicializarEstrategias(); // ⚠️ IMPORTANTE: Inicializar aquí
                Console.WriteLine($"\n✓ Distancia seleccionada: {config.MetodoDistancia}");
            }
            else
            {
                Console.WriteLine("\n❌ Opción inválida. Manteniendo configuración anterior.");
            }
        }

        private void ConfigurarNormalizacion()
        {
            Console.WriteLine("\n⚙️ Tipos de normalización disponibles:");
            string[] normas = { "MinMax", "ZScore", "Log" };
            for (int i = 0; i < normas.Length; i++)
                Console.WriteLine($"  {i + 1}. {normas[i]}");

            Console.Write("\n→ Seleccione una opción (1-3): ");
            if (int.TryParse(Console.ReadLine(), out int op) && op >= 1 && op <= 3)
            {
                config.MetodoNormalizacion = normas[op - 1];
                config.InicializarEstrategias(); // ⚠️ IMPORTANTE: Inicializar aquí
                Console.WriteLine($"\n✓ Normalización seleccionada: {config.MetodoNormalizacion}");
            }
            else
            {
                Console.WriteLine("\n❌ Opción inválida. Manteniendo configuración anterior.");
            }
        }

        private void ConfigurarPesos()
        {
            Console.WriteLine("\n⚖️ Configuración de pesos de variables");
            Console.Write("Ingrese los pesos separados por coma (ejemplo: 1,0.8,1.2): ");
            string entrada = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(entrada))
            {
                config.PesosVariables = null;
                Console.WriteLine("\n✓ Pesos reiniciados (por defecto = 1 para todas las variables).");
                return;
            }

            var partes = entrada.Split(',');
            double[] pesos = partes.Select(p => double.TryParse(p.Trim(), out var val) ? val : 1.0).ToArray();
            config.PesosVariables = pesos;
            Console.WriteLine($"\n✓ Pesos configurados: {string.Join(", ", pesos)}");
        }

        private void EjecutarClustering()
        {
            try
            {
                Console.WriteLine("\n╔══════════════════════════════════════════════════╗");
                Console.WriteLine("║          EJECUTANDO CLUSTERING JERÁRQUICO        ║");
                Console.WriteLine("╚══════════════════════════════════════════════════╝\n");

                // ✅ VALIDACIÓN 1: Verificar que hay archivo CSV configurado
                if (string.IsNullOrWhiteSpace(config.RutaCSV))
                {
                    Console.WriteLine("❌ ERROR: Debe configurar primero el archivo CSV (Opción 1).");
                    return;
                }

                // ✅ VALIDACIÓN 2: Verificar que el archivo existe
                if (!File.Exists(config.RutaCSV))
                {
                    Console.WriteLine($"❌ ERROR: El archivo no existe: {config.RutaCSV}");
                    Console.WriteLine("   Por favor, configure una ruta válida (Opción 1).");
                    return;
                }

                // ✅ VALIDACIÓN 3: Inicializar estrategias si no están configuradas
                if (config.DistanciaStrategy == null || config.NormalizacionStrategy == null)
                {
                    Console.WriteLine("⚠️  Las estrategias no están configuradas. Usando valores por defecto:");
                    Console.WriteLine("   • Distancia: Euclidiana");
                    Console.WriteLine("   • Normalización: MinMax\n");
                    
                    config.MetodoDistancia = "Euclidiana";
                    config.MetodoNormalizacion = "MinMax";
                    config.InicializarEstrategias();
                }

                // Confirmación para datasets grandes
                Console.WriteLine("⚠️  IMPORTANTE:");
                Console.WriteLine("   • El proceso puede tardar varios minutos según el tamaño del dataset");
                Console.WriteLine("   • NO cierre el programa durante el proceso");
                Console.WriteLine("   • Verá el progreso en pantalla\n");
                
                Console.Write("¿Desea continuar? (s/n): ");
                if (Console.ReadLine()?.ToLower() != "s")
                {
                    Console.WriteLine("\n❌ Operación cancelada por el usuario.");
                    return;
                }

                Console.WriteLine("\n" + new string('=', 50));
                var inicioTotal = DateTime.Now;

                // PASO 1: Cargar datos
                Console.WriteLine("\n📂 PASO 1/3: Cargando datos desde CSV...");
                var inicioCarga = DateTime.Now;
                
                controller.CargarDatos(config.RutaCSV);
                
                var tiempoCarga = (DateTime.Now - inicioCarga).TotalSeconds;
                Console.WriteLine($"✓ Datos cargados en {tiempoCarga:F2} segundos\n");

                // PASO 2: Normalizar datos
                Console.WriteLine("⚙️  PASO 2/3: Normalizando datos...");
                var inicioNorm = DateTime.Now;
                
                controller.NormalizarDatos();
                
                var tiempoNorm = (DateTime.Now - inicioNorm).TotalSeconds;
                Console.WriteLine($"✓ Normalización completada en {tiempoNorm:F2} segundos\n");

                // PASO 3: Ejecutar clustering
                Console.WriteLine("🚀 PASO 3/3: Ejecutando clustering jerárquico...");
                Console.WriteLine("   (Este proceso puede tardar. Por favor espere...)\n");
                var inicioCluster = DateTime.Now;
                
                controller.ProcesarCluster();
                
                var tiempoCluster = (DateTime.Now - inicioCluster).TotalSeconds;
                
                // Resumen final
                var tiempoTotal = (DateTime.Now - inicioTotal).TotalSeconds;
                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("✅ CLUSTERING COMPLETADO EXITOSAMENTE");
                Console.WriteLine(new string('=', 50));
                Console.WriteLine($"⏱️  Tiempos de ejecución:");
                Console.WriteLine($"   • Carga de datos:    {tiempoCarga:F2} s");
                Console.WriteLine($"   • Normalización:     {tiempoNorm:F2} s");
                Console.WriteLine($"   • Clustering:        {tiempoCluster:F2} s");
                Console.WriteLine($"   ─────────────────────────────────");
                Console.WriteLine($"   • TIEMPO TOTAL:      {tiempoTotal:F2} s ({tiempoTotal/60:F2} min)");
                Console.WriteLine(new string('=', 50));
                Console.WriteLine("\n💡 Ahora puede:");
                Console.WriteLine("   • Ver los resultados (Opción 6)");
                Console.WriteLine("   • Exportar a JSON (Opción 7)\n");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"\n❌ ERROR: Archivo no encontrado");
                Console.WriteLine($"   Detalle: {ex.Message}");
                Console.WriteLine($"   Verifique la ruta del archivo en la Opción 1.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ ERROR INESPERADO: {ex.Message}");
                Console.WriteLine($"\n📋 Detalles técnicos:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("\n💡 Sugerencias:");
                Console.WriteLine("   1. Verifique el formato del CSV");
                Console.WriteLine("   2. Asegúrese de tener suficiente memoria RAM");
                Console.WriteLine("   3. Intente con un dataset más pequeño");
            }
        }

        private void MostrarResultado()
        {
            try
            {
                var resultado = controller.ObtenerResultado();
                
                if (resultado == null)
                {
                    Console.WriteLine("\n⚠️  No hay resultados disponibles.");
                    Console.WriteLine("   Por favor, ejecute el clustering primero (Opción 5).\n");
                    return;
                }

                Console.WriteLine("\n╔══════════════════════════════════════════════════╗");
                Console.WriteLine("║           RESULTADOS DEL CLUSTERING              ║");
                Console.WriteLine("╚══════════════════════════════════════════════════╝\n");

                // Mostrar estadísticas
                resultado.MostrarEstadisticas();

                Console.WriteLine("\n¿Desea ver la estructura completa del dendrograma? (s/n): ");
                if (Console.ReadLine()?.ToLower() == "s")
                {
                    Console.WriteLine();
                    resultado.MostrarEstructura();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al mostrar resultado: {ex.Message}");
            }
        }

        private void ExportarResultado()
        {
            try
            {
                var resultado = controller.ObtenerResultado();
                
                if (resultado == null)
                {
                    Console.WriteLine("\n⚠️  No hay resultados para exportar.");
                    Console.WriteLine("   Por favor, ejecute el clustering primero (Opción 5).\n");
                    return;
                }

                Console.Write("\n💾 Ingrese la ruta de salida del archivo JSON");
                Console.Write("\n   (o Enter para '../../../datos/dendrograma.json'): ");
                
                string ruta = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(ruta))
                    ruta = "C:\\Users\\ignab\\Downloads\\dendrograma.json";

                controller.ExportarJson(ruta);
                
                Console.WriteLine($"\n✅ Dendrograma exportado correctamente");
                Console.WriteLine($"   Archivo: {ruta}");
                
                if (File.Exists(ruta))
                {
                    var info = new FileInfo(ruta);
                    Console.WriteLine($"   Tamaño: {info.Length / 1024.0:F2} KB");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al exportar: {ex.Message}");
            }
        }
    }
}