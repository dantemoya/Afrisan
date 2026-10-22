namespace Afrisan.Api.Models;

// Catálogo cerrado solicitado por HU-05; los valores deben validarse con AFRISAN.
public class CausaFalla
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool Activa { get; set; } = true;
}
