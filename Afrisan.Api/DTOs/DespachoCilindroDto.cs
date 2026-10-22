namespace Afrisan.Api.DTOs
{
    public class DespachoCilindroDto
    {
        public int? OrdenTrabajoId { get; set; }

        public string Responsable { get; set; } = string.Empty;

        [PesoKg]
        public decimal? PesoKg { get; set; }

        public string? Observaciones { get; set; }

        [System.ComponentModel.DataAnnotations.StringLength(1024)]
        public string? FotoPesajeRuta { get; set; }
    }
}
