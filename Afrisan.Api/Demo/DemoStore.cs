using Afrisan.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace Afrisan.Api.Demo;

// Estado de demostración por proceso: no registra DbContext, no abre conexiones
// y no escribe archivos. Los datos se restablecen al reiniciar la API.
public sealed class DemoStore
{
    public const string Password = "DemoAfrisan2026!";
    public object Gate { get; } = new();
    public string InstanciaId { get; } = Guid.NewGuid().ToString();
    public List<GasRefrigerante> Gases { get; } = [];
    public List<Cilindro> Cilindros { get; } = [];
    public List<OrdenTrabajo> Ordenes { get; } = [];
    public List<MovimientoInventario> Movimientos { get; } = [];
    public List<DemoUsuario> Usuarios { get; } = [];
    public PasswordHasher<Usuario> Hasher { get; } = new();
    public int NextCilindroId = 5, NextQr = 5, NextGasId = 4, NextOtId = 4, NextMovimientoId = 4;
    public static readonly string[] Roles = ["Administrador", "Jefatura", "Bodega", "Tecnico"];

    public DemoStore()
    {
        AddUser("demo-admin", "admin@afrisan.example", "Administrador Demo", "Administrador", Password);
        AddUser("demo-jefatura", "jefatura@afrisan.example", "Jefatura Demo", "Jefatura", Password);
        AddUser("demo-bodega", "bodega@afrisan.example", "Bodega Demo", "Bodega", Password);
        AddUser("demo-tecnico", "tecnico@afrisan.example", "Técnico Demo", "Tecnico", Password);
        Gases.AddRange([
            new() { Id = 1, Codigo = "R-134a", Nombre = "Refrigerante R-134a", Descripcion = "Catálogo simulado" },
            new() { Id = 2, Codigo = "R-410A", Nombre = "Refrigerante R-410A", Descripcion = "Catálogo simulado" },
            new() { Id = 3, Codigo = "R-404A", Nombre = "Refrigerante R-404A", Descripcion = "Ejemplo inactivo", Activo = false }
        ]);
        // Los GWP quedan sin configurar: estos datos de ejemplo no son un inventario ambiental.
        var now = DateTime.UtcNow;
        Ordenes.AddRange([
            new() { Id = 1, Codigo = "OT-000001", Descripcion = "Servicio de ejemplo finalizado", Activa = false, FechaCreacion = now.AddDays(-3), FechaCierre = now.AddDays(-2) },
            new() { Id = 2, Codigo = "OT-000002", Descripcion = "Mantenimiento en sucursal de demostración", FechaCreacion = now.AddDays(-1), TecnicoAsignadoId = "demo-tecnico" },
            new() { Id = 3, Codigo = "OT-000003", Descripcion = "Orden disponible para practicar un despacho", FechaCreacion = now }
        ]);
        Cilindros.AddRange([
            Cylinder(1, 1, 10, 6, 14, "Disponible", now.AddDays(-5)),
            Cylinder(2, 2, 12, 7, 17, "Despachado", now.AddDays(-4)),
            Cylinder(3, 1, 10, 5, 15, "Disponible", now.AddDays(-1)),
            Cylinder(4, 2, 12, null, 12, "Disponible", now)
        ]);
        Movimientos.AddRange([
            new() { Id = 1, CilindroId = 1, OrdenTrabajoId = 1, UsuarioId = "demo-bodega", TipoMovimiento = "Despacho", PesoKg = 16, Responsable = "Técnico Demo", FechaMovimiento = now.AddDays(-3), Observaciones = "Movimiento simulado" },
            new() { Id = 2, CilindroId = 1, OrdenTrabajoId = 1, UsuarioId = "demo-bodega", DespachoId = 1, TipoMovimiento = "Retorno", PesoKg = 14, Responsable = "Técnico Demo", FechaMovimiento = now.AddDays(-2), Observaciones = "Retorno simulado" },
            new() { Id = 3, CilindroId = 2, OrdenTrabajoId = 2, UsuarioId = "demo-bodega", CustodiaAbierta = true, TipoMovimiento = "Despacho", PesoKg = 17, Responsable = "Técnico Demo", FechaMovimiento = now.AddHours(-3), Observaciones = "Pendiente de retorno" }
        ]);
    }

    private Cilindro Cylinder(int id, int gas, decimal capacidad, decimal? tara, decimal peso, string estado, DateTime fecha)
        => new() { Id = id, CodigoQr = $"CIL-{id:D4}", NumeroSerie = $"DEMO-{id:D4}", GasRefrigeranteId = gas,
            GasRefrigerante = Gases.Single(g => g.Id == gas), TipoRefrigerante = Gases.Single(g => g.Id == gas).Codigo,
            CapacidadKg = capacidad, TaraKg = tara, PesoActualKg = peso, Estado = estado, FechaRegistro = fecha };

    public DemoUsuario AddUser(string id, string email, string nombre, string rol, string password)
    {
        var account = new Usuario { Id = id, UserName = email, Email = email, NombreCompleto = nombre,
            RolId = "demo-rol-" + rol, Activo = true, EmailConfirmed = true };
        account.PasswordHash = Hasher.HashPassword(account, password);
        var user = new DemoUsuario { Cuenta = account, Rol = rol };
        Usuarios.Add(user);
        return user;
    }
}

public sealed class DemoUsuario
{
    public required Usuario Cuenta { get; init; }
    public required string Rol { get; init; }
    public int IntentosFallidos { get; set; }
    public DateTime? BloqueadoHasta { get; set; }
}
