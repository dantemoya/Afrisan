namespace Afrisan.Api.Models;

// Una carga sobre un equipo con un cilindro. Una OT puede tener muchas cargas;
// los pesajes de despacho y retorno pertenecen a MovimientoInventario.
public class Intervencion
{
    public int Id { get; set; }
    // El cliente offline deberá reutilizar el mismo identificador al reintentar.
    public Guid IdOperacion { get; set; } = Guid.NewGuid();
    public int OrdenTrabajoId { get; set; }
    public int CilindroId { get; set; }
    // Identifica el ciclo de custodia exacto, no solo el último uso del cilindro.
    public int DespachoId { get; set; }
    public int SucursalId { get; set; }
    public int EquipoId { get; set; }
    public int CausaFallaId { get; set; }
    public string TecnicoId { get; set; } = string.Empty;
    public string? AutorizadorId { get; set; }
    public DateTime FechaIntervencion { get; set; } = DateTime.UtcNow;
    public decimal KgUtilizados { get; set; }
    public decimal? PesoRestanteBrutoKg { get; set; }
    // Evidencia HU-06 en terreno, distinta de las fotos de los pesajes de bodega.
    public string? FotoBalanzaRuta { get; set; }
    public string EstadoIntervencion { get; set; } = "Registrada";
    public string? PlanCorreccion { get; set; }
    public string EstadoSemaforo { get; set; } = "Pendiente";
    // Se conserva el factor utilizado para no recalcular el pasado con otro catálogo.
    public decimal? FactorGwpAplicado { get; set; }
    public string? FuenteGwpAplicada { get; set; }
    public string? MetodologiaGwpAplicada { get; set; }
    public decimal? PrecioKgAplicado { get; set; }
    public string? Moneda { get; set; }
    // Los kg cargados no son automáticamente emisiones. Null significa que
    // la masa emitida aún no se declaró/estimó con el método acordado.
    public decimal? KgEmitidos { get; set; }
    public decimal? EmisionesCo2eToneladas { get; private set; }
    public decimal? CostoTotal { get; private set; }
    public string EstadoSincronizacion { get; set; } = "Sincronizado";
    public string? DispositivoId { get; set; }
    public DateTime? FechaCreacionLocal { get; set; }
    public DateTime FechaRecepcion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaCierre { get; set; }
    public string? Observaciones { get; set; }
}
