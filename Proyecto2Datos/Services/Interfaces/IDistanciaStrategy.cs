using Proyecto2Datos.Model;

namespace Proyecto2Datos.Services.Distancias
{
    public interface IDistanciaStrategy
    {
        float Calcular(Vector v1, Vector v2);
        string Nombre { get; }
    }
}