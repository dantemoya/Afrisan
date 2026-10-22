using System.ComponentModel.DataAnnotations;

namespace Afrisan.Api.DTOs
{
    public class GasRefrigeranteDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Codigo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        [Range(typeof(decimal), "0", "999999999.999")]
        public decimal? FactorGwp { get; set; }

        [StringLength(300)]
        public string? FuenteGwp { get; set; }

        [StringLength(100)]
        public string? MetodologiaGwp { get; set; }
    }
}
