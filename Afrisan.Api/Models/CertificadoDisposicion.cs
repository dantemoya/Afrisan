namespace Afrisan.Api.Models;

public class CertificadoDisposicion
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string EmpresaEmisora { get; set; } = "Regener";
    public DateOnly FechaEmision { get; set; }
    public string RutaDocumento { get; set; } = string.Empty;
    public string UsuarioId { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
