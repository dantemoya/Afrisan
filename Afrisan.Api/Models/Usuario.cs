using Microsoft.AspNetCore.Identity;

namespace Afrisan.Api.Models
{
    public class Usuario : IdentityUser
    {
        public string NombreCompleto { get; set; } = string.Empty;

        // Rol único por cuenta. Se conservan las claves de texto de Identity y sus hashes.
        public string? RolId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public IdentityRole? Rol { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
