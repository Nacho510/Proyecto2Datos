namespace Proyecto2Datos.Model
{
    public class DataPoint
    {
        public int Id { get; set; }
        public Vector VectorDatos { get; set; }
        
        public Vector Vector
        {
            get => VectorDatos;
            set => VectorDatos = value;
        }

        public List<string>? Categorias { get; set; }

        public DataPoint(int id, Vector vector)
        {
            Id = id;
            VectorDatos = vector;
        }

        public DataPoint(string id, Vector vector, List<string>? categorias = null)
        {
            // Intentar parsear como int, si falla usar hash
            Id = int.TryParse(id, out int idNum) ? idNum : Math.Abs(id.GetHashCode());
            VectorDatos = vector;
            Categorias = categorias;
        }

        public DataPoint() { }
    }
}