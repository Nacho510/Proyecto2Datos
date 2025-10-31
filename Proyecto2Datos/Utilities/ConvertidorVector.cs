using Proyecto2Datos.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Proyecto2Datos.Utilities
{
    public static class ConvertidorVector
    {
        
        public static Vector ConvertirFila(List<string> valores, List<int> indicesNumericos, 
            List<int> indicesCategoricos, Dictionary<int, Dictionary<string, int>> mapasCategorias)
        {
            var componentes = new List<float>();

            for (int i = 0; i < valores.Count; i++)
            {
                string valor = valores[i].Trim();

                // Si la columna es numérica
                if (indicesNumericos.Contains(i))
                {
                    if (float.TryParse(valor, NumberStyles.Float, CultureInfo.InvariantCulture, out float num))
                        componentes.Add(num);
                    else
                        componentes.Add(0f);
                }
                // Si la columna es categórica
                else if (indicesCategoricos.Contains(i))
                {
                    if (!mapasCategorias.ContainsKey(i))
                        mapasCategorias[i] = new Dictionary<string, int>();

                    var mapa = mapasCategorias[i];

                    if (!mapa.ContainsKey(valor))
                        mapa[valor] = mapa.Count;

                    int indice = mapa[valor];
                    // One-hot encoding
                    for (int j = 0; j < mapa.Count; j++)
                        componentes.Add(j == indice ? 1f : 0f);
                }
            }
            var vector = new Vector(componentes.Count);
            for (int i = 0; i < componentes.Count; i++)
            {
                vector[i] = componentes[i];
            }
            return vector;
        }
        
        public static Vector ConvertirNumerico(List<float> valores)
        {
            var vector = new Vector(valores.Count);
            for (int i = 0; i < valores.Count; i++)
            {
                vector[i] = valores[i];
            }
            return vector;
        }
        
        public static Vector ConvertirLineaCSV(string linea, char separador = ',')
        {
            var partes = linea.Split(separador);
            var lista = new List<float>();

            foreach (var p in partes)
            {
                if (float.TryParse(p.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float num))
                    lista.Add(num);
                else
                    lista.Add(0f);
            }

            var vector = new Vector(lista.Count);
            for (int i = 0; i < lista.Count; i++)
            {
                vector[i] = lista[i];
            }
            return vector;
        }
    }
}