using Proyecto2Datos.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Proyecto2Datos.Utilities
{
    public class LectorCSV
    {
        public List<DataPoint> LeerArchivo(string ruta)
        {
            if (!File.Exists(ruta))
                throw new FileNotFoundException($"No se encontró el archivo CSV en: {ruta}");

            var lineas = File.ReadAllLines(ruta)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToArray();
            
            if (lineas.Length <= 1)
                throw new Exception("El archivo CSV está vacío o solo contiene encabezado.");

            var encabezado = lineas[0].Split(',');
            var dataPoints = new List<DataPoint>();

            // Detectar columnas numéricas
            string[] primeraFila = lineas[1].Split(',');
            List<int> columnasNumericas = new();
            List<int> columnasTexto = new();

            for (int i = 1; i < primeraFila.Length; i++)
            {
                if (double.TryParse(primeraFila[i].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                    columnasNumericas.Add(i);
                else
                    columnasTexto.Add(i);
            }

            Console.WriteLine($"📊 Detectadas {columnasNumericas.Count} columnas numéricas y {columnasTexto.Count} categóricas");

            for (int i = 1; i < lineas.Length; i++)
            {
                var valores = lineas[i].Split(',');
                
                if (valores.Length < 2)
                {
                    Console.WriteLine($"⚠ Fila {i} ignorada: datos insuficientes");
                    continue;
                }

                string id = valores[0].Trim();
                if (string.IsNullOrWhiteSpace(id))
                    id = $"Punto_{i}";

                List<float> componentes = new(); // ✅ CORREGIDO: float en vez de double
                List<string> categorias = new();

                foreach (var col in columnasNumericas)
                {
                    if (col < valores.Length)
                    {
                        if (float.TryParse(valores[col], NumberStyles.Any, CultureInfo.InvariantCulture, out float val))
                            componentes.Add(val);
                        else
                            componentes.Add(0f);
                    }
                }

                foreach (var col in columnasTexto)
                {
                    if (col < valores.Length)
                        categorias.Add(valores[col].Trim());
                }

                if (componentes.Count > 0)
                {
                    // ✅ CORRECCIÓN CRÍTICA
                    Vector v = new Vector(componentes.Count); // Pasa int
                    for (int k = 0; k < componentes.Count; k++)
                        v[k] = componentes[k]; // Asigna cada componente

                    dataPoints.Add(new DataPoint(id, v, categorias.Count > 0 ? categorias : null));
                }
            }

            if (dataPoints.Count == 0)
                throw new Exception("No se pudo cargar ningún punto de datos válido del CSV.");

            Console.WriteLine($"✓ Se leyeron {dataPoints.Count} filas válidas.");
            Console.WriteLine($"  • Dimensión de vectores: {dataPoints[0].Vector.Dimension}");

            return dataPoints;
        }
    }
}