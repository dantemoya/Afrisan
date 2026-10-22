namespace Afrisan.Api.Models;

// El cliente de una intervención se obtiene a través de su sucursal.
public class Sucursal
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
