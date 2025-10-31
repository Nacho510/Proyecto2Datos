using Proyecto2Datos.View;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var menu = new MenuConsola();
        menu.Iniciar();
    }
}