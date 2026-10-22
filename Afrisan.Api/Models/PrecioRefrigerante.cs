namespace Afrisan.Api.Models;

// Cada variación de tarifa crea una fila nueva. El precio aplicado se copia a la
// intervención para conservar los costos históricos aunque cambie el catálogo.
public class PrecioRefrigerante
{
    public int Id { get; set; }
    public int GasRefrigeranteId { get; set; }
    public DateTime VigenteDesde { get; set; }
    public decimal PrecioKg { get; set; }
    public string Moneda { get; set; } = "CLP";
}
