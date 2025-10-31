using Proyecto2Datos.Model;
using System.Collections.Generic;

namespace Proyecto2Datos.Services.Normalizacion
{
    public interface INormalizacionStrategy
    {
        void Normalizar(List<DataPoint> puntos);
        string Nombre { get; }
    }
}