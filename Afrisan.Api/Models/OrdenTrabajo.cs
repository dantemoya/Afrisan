namespace Afrisan.Api.Models
{
    public class OrdenTrabajo
    {
        public int Id { get; set; }

        // Código oficial de la Orden de Trabajo.
        // No será inventado desde el formulario de despacho:
        // el usuario deberá seleccionar una OT existente.
        public string Codigo { get; set; } = string.Empty;

        public string Origen { get; set; } = "Local";

        public string? CodigoExterno { get; set; }

        public int? SucursalId { get; set; }

        public string? TecnicoAsignadoId { get; set; }

        public string? AutorizadorId { get; set; }

        // La OT agrupa varios movimientos e intervenciones; cada uso tiene su cilindro y tarifa.
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<Intervencion> Intervenciones { get; set; } = new List<Intervencion>();

        public DateTime? FechaCierre { get; set; }

        public string? Descripcion { get; set; }

        public bool Activa { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public ICollection<MovimientoInventario> Movimientos
        {
            get;
            set;
        } = new List<MovimientoInventario>();
    }
}
