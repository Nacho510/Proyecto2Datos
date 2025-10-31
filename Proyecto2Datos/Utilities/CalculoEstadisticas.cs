namespace Proyecto2Datos.Utilities;

public class CalculoEstadisticas
{
    public double Media(double[] datos)
        => datos.Average();

    public double DesviacionEstandar(double[] datos)
    {
        double media = Media(datos);
        double suma = datos.Sum(d => Math.Pow(d - media, 2));
        return Math.Sqrt(suma / datos.Length);
    }

    public double Min(double[] datos) => datos.Min();
    public double Max(double[] datos) => datos.Max();
}