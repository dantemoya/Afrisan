namespace Afrisan.Api.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Rut { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; } = true;
}
