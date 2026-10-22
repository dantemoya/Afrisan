namespace Afrisan.Api.Models;

public class RegistroAuditoria
{
    public long Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public int? IntervencionId { get; set; }
    public string Accion { get; set; } = string.Empty;
    public string Entidad { get; set; } = string.Empty;
    public string RegistroId { get; set; } = string.Empty;
    // Documentos JSON; nunca deben contener contraseñas ni tokens.
    public string? ValoresAnteriores { get; set; }
    public string? ValoresNuevos { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
