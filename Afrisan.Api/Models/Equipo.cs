namespace Afrisan.Api.Models;

// HU-05 y HU-07 distinguen cada equipo y su capacidad dentro de una sucursal.
public class Equipo
{
    public int Id { get; set; }
    public int SucursalId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal CapacidadNominalKg { get; set; }
    public int? GasRefrigeranteId { get; set; }
    public bool Activo { get; set; } = true;
}
